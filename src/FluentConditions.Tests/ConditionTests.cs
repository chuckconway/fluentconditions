using FluentAssertions;
using Xunit;

namespace FluentConditions.Tests
{
    public class ConditionTests
    {
        [Fact]
        public void check_run_always_executes()
        {
            var hasRun = false;

            Conditions
                .Run(() => hasRun = true);

            hasRun.Should().BeTrue("Run() should have run.");
        }

        [Fact]
        public void success_should_not_execute_when_one_or_more_evaluations_are_false()
        {
            var hasRun = false;

            Conditions
                .When(() => true)
                .ThenWhen(() => false)
                .ThenWhen(() => true)
                    .Success(() => hasRun = true);

            hasRun.Should().BeFalse("Success() should not have executed.");
        }
        
        [Fact]
        public void success_should_only_execute_when_all_evaluations_are_true()
        {
            var hasRun = false;

            Conditions
                .When(() => true)
                .ThenWhen(() => true)
                .ThenWhen(() => true)
                    .Success(() => hasRun = true);

            hasRun.Should().BeTrue("Success() should have executed.");
        }
        
        [Fact]
        public void check_run_always_execute()
        {
            var hasRun = false;

            Conditions
                .When(() => false)
                .ThenWhen(() => false)
                .ThenWhen(() => false)
                    .Always(() => hasRun = true);

            hasRun.Should().BeTrue("Always() should have executed.");
        }
        
        [Fact]
        public void failure_should_have_executed()
        {
            var hasRun = false;

            Conditions
                .When(() => false)
                .Failure(() => hasRun = true);

            hasRun.Should().BeTrue("Failure() should have executed.");
        }
        
        [Fact]
        public void failure_should_have_not_executed()
        {
            var hasRun = false;

            Conditions
                .When(() => false)
                .Failure(() => {})
                .Failure(() => hasRun = true);

            hasRun.Should().BeFalse("Failure() should not have executed.");
        }
    }
}