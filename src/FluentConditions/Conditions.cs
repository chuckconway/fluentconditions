using System;

namespace FluentConditions
{
    public class Conditions
    {
        private bool _isValid = true;
        private bool _hasFailedRun;

        /// <summary>
        /// Begin a new set of conditions
        /// </summary>
        /// <returns></returns>
        public static Conditions When(Func<bool> evaluate)
        {
            return new Conditions()
                .ThenWhen(evaluate);
        }

        /// <summary>
        /// Evaluate the condition, when it's true, move to the next condition
        /// </summary>
        /// <param name="evaluate"></param>
        /// <returns></returns>
        public Conditions ThenWhen(Func<bool> evaluate)
        {
            if (_isValid)
            {
                _isValid = evaluate();
            }

            return this;
        }

        /// <summary>
        /// If there are no upstream failures, a successful condition will run, including when Success is the first call. The default state of a Condition is 'true'
        /// </summary>
        /// <param name="successAction"></param>
        /// <returns></returns>
        public Conditions Success(Action successAction)
        {
            if (_isValid)
            {
                successAction();
            }

            return this;
        }

        /// <summary>
        /// Runs regardless if of the pass/fail of the evaluations.
        /// </summary>
        /// <param name="run"></param>
        /// <returns></returns>
        public Conditions Always(Action run)
        {
            run();
            
            return this;
        }
        
        /// <summary>
        /// Runs regardless if of the pass/fail of the evaluations.
        /// </summary>
        /// <param name="run"></param>
        /// <returns></returns>
        public static Conditions Run(Action run)
        {
            return new Conditions()
                .Always(run);
        }

        /// <summary>
        /// Executes when condition has failed
        /// </summary>
        /// <param name="failedAction"></param>
        /// <returns></returns>
        public Conditions Failure(Action failedAction)
        {
            if (!_isValid && !_hasFailedRun)
            {
                failedAction();
                _hasFailedRun = true;
            }

            return this;
        }
    }
}