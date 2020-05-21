# Fluent Conditions

Fluent Condition is designed to simplify nested if-statements.

In many systems nested if-statements are a honey-pot of defects, to fix these defect the must be understood, but the logic can be difficult to follow, debug or change. Fluent Conditions aims to help reduce the pain by providing a fluent syntax that helps express requirements in a readable syntax. 

Well designed systems don't have many nested if-statements, our goal isn't to replace well designed system, but to make systems on the road to recovery eaiser to read.



For example, this:

```c#
var errors = new List<ErrorResult>();

if (validation.IsValidId(row.RowId))
{

  // check for hash values
  if (row.VerifyHash(row.RowHash))
  {
    
    if (validation.IsValidStatus(row.Status))  //check that the status value is one of the expected values.                        
    {
      //Save to the database
      repository.Update(row);
    }
    else
    {
      errors.Add(new ErrorResult("The status is not a valid value for rowId " + row.RowId));
    }
  }
  else
  {
    // Error, values have changed.
    errors.Add(new ErrorResult("The values of the row have changed since you retrieved your data, please refresh your data."));
  }
}
else
{
  // Error, Duplicate found
  errors.Add(new ErrorResult("We found another row with the same RowId, please verify your data is correct."));
}

```



Becomes this:

```c#
var errors = new List<ErrorResult>();

Conditions.
  When(() => validation.IsValidId(row.RowId))
  	.Failure(() => errors.Add(new ErrorResult("We found another row with the same RowId, please verify your data is correct.")))
  .ThenWhen(() => row.VerifyHash(row.RowHash))
  	.Failure(() => errors.Add(new ErrorResult("The values of the row have changed since you retrieved your data, please refresh your data.")))
  .ThenWhen(() => validation.IsValidStatus(row.Status))
  	.Success(() => repository.Update(row))
  	.Failure(() => errors.Add(new ErrorResult("The status is not a valid value for rowId " + row.RowId)));
```



## API

| Methods     | Description                                                  |
| ----------- | ------------------------------------------------------------ |
| .When()     | is a static method, is called once, takes a `Func<bool>`, returns a new `Conditions` instance. |
| .Run()      | is a static method, takes an `Action`, returns a new `Conditions` instance. |
| .ThenWhen() | is an instance method and can be chained many times as needed. This method takes a single in parameter: `Func<bool>`. Once the condition is in a failure state, subsequent .ThenWhen() calls are ignored. |
| .Success()  | is an instance method and can be called as many times as needed. This method takes a single in parameter: `Action`. `.Success()` executes as long as the condition is in a true state. As soon as `.When()` or `.ThenWhen()` enters a failure state (i.e. a condition evaluating to `false`) `.Success()` is no longer executed. |
| .Failure()  | is an instance method. This method takes a single in parameter: `Action`. **It is run once**, right after a failed (an evaluation of `false`) `.ThenWhen()` or `.When()`. |
| .Always()   | is an instance method. This method takes a single in parameter: `Action`. `.Always()` executes regardless of the state (`true` or `false`) of the evaluations. It can be chained indefinitely. |



### Examples of various states



```c#
Conditions
  .When(() => true)
	  .Success(() => { }) // run as along as there is not an upstream failure
	  .Failure(() => { }) // Runs once after the first failure.
	  .Always(() => { })  // Always runs, regardless of success/failure state
  .ThenWhen(() => true)
	  .Success(() => { }) // will run
	  .Failure(() => {})  // won't run
  .ThenWhen(() => false) 
	  .Success(() => { }) // won't run
	  .Failure(() => { }) // will run
  .ThenWhen(() => false) 
	  .Success(() => { }) // won't run
	  .Failure(() => { }) // won't run
  .ThenWhen(() => true) 
	  .Success(() => { }) // won't run
	  .Failure(() => { }) // won't run
	  .Always(() => { })  // will run
  .ThenWhen(() => true) 
  .Always(() => { });  // will run
```

