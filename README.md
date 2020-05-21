# Fluent Conditions

The goal of this library, isn't to replace good design, but to make bad design easier to read. In a well designed system, the need for nested if statements is rare.



For example, this:



​                var errors = new List<ErrorResult>();

​                

​                if (validation.IsValidId(row.RowId))

​                {

​                    // check for hash values

​                    if (row.VerifyHash(row.RowHash))

​                    {

​                        if (validation.IsValidStatus(row.Status))  //check that the status value is one of the expected values.                        

​                        {

​                            //Save to the database

​                            repository.Update(row);

​                        }

​                        else

​                        {

​                           errors.Add(new ErrorResult("The status is not a valid value for rowId " + row.RowId));

​                        }

​                    }

​                    else

​                    {

​                        // Error, values have changed.

​                        errors.Add(new ErrorResult("The values of the row have changed since you retrieved your data, please refresh your data."));

​                    }

​                }

​                else

​                {

​                    // Error, Duplicate found

​                    errors.Add(new ErrorResult("We found another row with the same RowId, please verify your data is correct."));

​                }



Becomes this:



​                var errors = new List<ErrorResult>();



​                Conditions.

​                    When(() => validation.IsValidId(row.RowId))

​                        .Failure(() => errors.Add(new ErrorResult("We found another row with the same RowId, please verify your data is correct.")))

​                    ThenWhen(() => row.VerifyHash(row.RowHash))

​                        .Failure(() => errors.Add(new ErrorResult("The values of the row have changed since you retrieved your data, please refresh your data.")))

​                    ThenWhen(() => validation.IsValidStatus(row.Status))

​                        .Success(() => repository.Update(row))

​                        .Failure(() => errors.Add(new ErrorResult("The status is not a valid value for rowId " + row.RowId)))









Example



​                Conditions

​                    .When(() => true)

​                        .Success(() => { }) // run as along as there is not an upstream failure

​                        .Failure(() => { }) // Runs once after the first failure.

​                        .Always(() => { })  // Always runs, regardless of success/failure state.

​                    .ThenWhen(() => true)

​                        .Success(() => { }) // will run

​                        .Failure(() => {})  // won't run

​                    .ThenWhen(() => false) 

​                        .Success(() => { }) // won't run

​                        .Failure(() => { }) // will run

​                    .ThenWhen(() => false) 

​                        .Success(() => { }) // won't run

​                        .Failure(() => { }) // won't run

​                    .ThenWhen(() => true) 

​                        .Success(() => { }) // won't run

​                        .Failure(() => { }) // won't run

​                        .Always(() => { })  // will run

​                    .ThenWhen(() => true) 

​                        .Always(() => { })  // will run