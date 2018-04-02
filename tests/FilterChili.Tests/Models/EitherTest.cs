using System;
using FluentAssertions;
using GravityCTRL.FilterChili.Models;
using JetBrains.Annotations;
using Xunit;

namespace GravityCTRL.FilterChili.Tests.Models
{
    public sealed class EitherTest
    {
        private bool _isLeft;
        private bool _isRight;

        [Fact]
        public void Should_Create_Left_Either_Instance()
        {
            var testInstance = Either.Left<int, double>(3);
            testInstance.Should().BeOfType<Left<int, double>>();

            testInstance.TryGetLeft(out var left).Should().BeTrue();
            left.Should().Be(3);

            testInstance.TryGetRight(out var right).Should().BeFalse();
            right.Should().Be(0.0);

            testInstance.Match(LeftFunction, RightFunction).Should().Be("left");
            testInstance.Match(LeftAction, RightAction);

            _isLeft.Should().BeTrue();
            _isRight.Should().BeFalse();
        }

        [Fact]
        public void Should_Create_Right_Either_Instance()
        {
            var testInstance = Either.Right<int, double>(5.0);
            testInstance.Should().BeOfType<Right<int, double>>();

            testInstance.TryGetLeft(out var left).Should().BeFalse();
            left.Should().Be(0);

            testInstance.TryGetRight(out var right).Should().BeTrue();
            right.Should().Be(5.0);

            testInstance.Match(LeftFunction, RightFunction).Should().Be("right");
            testInstance.Match(LeftAction, RightAction);

            _isLeft.Should().BeFalse();
            _isRight.Should().BeTrue();
        }

        [Fact]
        public void Should_Throw_ArgumentException_When_Either_Is_Not_Left_Or_Right()
        {
            var either = new InvalidEither();
            Action action = () => either.Match(LeftFunction, RightFunction);
            action.Should().Throw<ArgumentException>().WithMessage("either");
        }

        [Fact]
        public void Should_Create_Left_Either_Instance_Implicitly()
        {
            Either<int, double> testInstance = 3;
            testInstance.Should().BeOfType<Left<int, double>>();

            testInstance.TryGetLeft(out var left).Should().BeTrue();
            left.Should().Be(3);

            testInstance.TryGetRight(out var right).Should().BeFalse();
            right.Should().Be(0.0);

            testInstance.Match(LeftFunction, RightFunction).Should().Be("left");
            testInstance.Match(LeftAction, RightAction);

            _isLeft.Should().BeTrue();
            _isRight.Should().BeFalse();
        }

        [Fact]
        public void Should_Create_Right_Either_Instance_Implicitly()
        {
            Either<int, double> testInstance = 5.0;
            testInstance.Should().BeOfType<Right<int, double>>();

            testInstance.TryGetLeft(out var left).Should().BeFalse();
            left.Should().Be(0);

            testInstance.TryGetRight(out var right).Should().BeTrue();
            right.Should().Be(5.0);

            testInstance.Match(LeftFunction, RightFunction).Should().Be("right");
            testInstance.Match(LeftAction, RightAction);

            _isLeft.Should().BeFalse();
            _isRight.Should().BeTrue();
        }

        [NotNull]
        private string LeftFunction(int value)
        {
            value.Should().Be(3);
            return "left";
        }

        [NotNull]
        private string RightFunction(double value)
        {
            value.Should().Be(5.0);
            return "right";
        }

        [NotNull]
        private void LeftAction(int value)
        {
            value.Should().Be(3);
            _isLeft = true;
        }

        [NotNull]
        private void RightAction(double value)
        {
            value.Should().Be(5.0);
            _isRight = true;
        }

        private class InvalidEither : Either<int, double> {}
    }
}
