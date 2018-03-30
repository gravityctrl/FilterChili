using System;
using System.Linq.Expressions;
using FluentAssertions;
using GravityCTRL.FilterChili.Expressions;
using GravityCTRL.FilterChili.Search;
using GravityCTRL.FilterChili.Tests.TestSupport.Models;
using JetBrains.Annotations;
using Xunit;

namespace GravityCTRL.FilterChili.Tests.Search
{
    public sealed class SearchSpecificationTest
    {
        private readonly SearchSpecification<GenericSource> _testInstance;

        public SearchSpecificationTest()
        {
            _testInstance = new SearchSpecification<GenericSource>(source => source.String);
        }

        [Fact]
        public void Should_Set_Name_On_Calling_UseName()
        {
            _testInstance.UseName("Foobert");
            _testInstance.Name.Should().Be("Foobert");
        }

        [Fact]
        public void Should_Initialize_Instance_Correctly()
        {
            var testEntity = new GenericSource { String = "Das ist ein Test" };
            _testInstance.IncludeAcceptsMultipleInputs.Should().BeTrue();

            CreateLambdaFunction(_testInstance.IncludeExpression("Das ist ein Test")).Invoke(testEntity).Should().BeTrue();
            CreateLambdaFunction(_testInstance.IncludeExpression("Test")).Invoke(testEntity).Should().BeTrue();
            CreateLambdaFunction(_testInstance.IncludeExpression("Täst")).Invoke(testEntity).Should().BeFalse();
            CreateLambdaFunction(_testInstance.IncludeExpression("Tesd")).Invoke(testEntity).Should().BeFalse();

            CreateLambdaFunction(_testInstance.ExcludeExpression("Das ist ein Test")).Invoke(testEntity).Should().BeTrue();
            CreateLambdaFunction(_testInstance.ExcludeExpression("Test")).Invoke(testEntity).Should().BeTrue();
            CreateLambdaFunction(_testInstance.ExcludeExpression("Täst")).Invoke(testEntity).Should().BeFalse();
            CreateLambdaFunction(_testInstance.ExcludeExpression("Tesd")).Invoke(testEntity).Should().BeFalse();
        }

        [Fact]
        public void Should_Provide_Correct_Expression_When_Calling_UseContains()
        {
            var testEntity = new GenericSource { String = "Das ist ein Test" };
            _testInstance.IncludeAcceptsMultipleInputs.Should().BeTrue();

            CreateLambdaFunction(_testInstance.IncludeExpression("Das ist ein Test")).Invoke(testEntity).Should().BeTrue();
            CreateLambdaFunction(_testInstance.IncludeExpression("Test")).Invoke(testEntity).Should().BeTrue();
            CreateLambdaFunction(_testInstance.IncludeExpression("Täst")).Invoke(testEntity).Should().BeFalse();
            CreateLambdaFunction(_testInstance.IncludeExpression("Tesd")).Invoke(testEntity).Should().BeFalse();

            CreateLambdaFunction(_testInstance.ExcludeExpression("Das ist ein Test")).Invoke(testEntity).Should().BeTrue();
            CreateLambdaFunction(_testInstance.ExcludeExpression("Test")).Invoke(testEntity).Should().BeTrue();
            CreateLambdaFunction(_testInstance.ExcludeExpression("Täst")).Invoke(testEntity).Should().BeFalse();
            CreateLambdaFunction(_testInstance.ExcludeExpression("Tesd")).Invoke(testEntity).Should().BeFalse();
        }

        [Fact]
        public void Should_Provide_Correct_Expression_When_Calling_UseEquals()
        {
            _testInstance.UseEquals();
            _testInstance.IncludeAcceptsMultipleInputs.Should().BeFalse();

            var testEntity = new GenericSource { String = "Das ist ein Test" };

            CreateLambdaFunction(_testInstance.IncludeExpression("Das ist ein Test")).Invoke(testEntity).Should().BeTrue();
            CreateLambdaFunction(_testInstance.IncludeExpression("Test")).Invoke(testEntity).Should().BeFalse();
            CreateLambdaFunction(_testInstance.IncludeExpression("Täst")).Invoke(testEntity).Should().BeFalse();
            CreateLambdaFunction(_testInstance.IncludeExpression("Tesd")).Invoke(testEntity).Should().BeFalse();

            CreateLambdaFunction(_testInstance.ExcludeExpression("Das ist ein Test")).Invoke(testEntity).Should().BeTrue();
            CreateLambdaFunction(_testInstance.ExcludeExpression("Test")).Invoke(testEntity).Should().BeFalse();
            CreateLambdaFunction(_testInstance.ExcludeExpression("Täst")).Invoke(testEntity).Should().BeFalse();
            CreateLambdaFunction(_testInstance.ExcludeExpression("Tesd")).Invoke(testEntity).Should().BeFalse();
        }

        [Fact]
        public void Should_Provide_Correct_Expression_When_Calling_UseSoundex()
        {
            _testInstance.UseSoundex();
            _testInstance.IncludeAcceptsMultipleInputs.Should().BeTrue();

            var testEntity = new GenericSource { String = "Das ist ein Test" };

            CreateLambdaFunction(_testInstance.IncludeExpression("Das ist ein Test")).Invoke(testEntity).Should().BeTrue();
            CreateLambdaFunction(_testInstance.IncludeExpression("Test")).Invoke(testEntity).Should().BeTrue();
            CreateLambdaFunction(_testInstance.IncludeExpression("Täst")).Invoke(testEntity).Should().BeTrue();
            CreateLambdaFunction(_testInstance.IncludeExpression("Desd")).Invoke(testEntity).Should().BeFalse();

            CreateLambdaFunction(_testInstance.ExcludeExpression("Das ist ein Test")).Invoke(testEntity).Should().BeTrue();
            CreateLambdaFunction(_testInstance.ExcludeExpression("Test")).Invoke(testEntity).Should().BeTrue();
            CreateLambdaFunction(_testInstance.ExcludeExpression("Täst")).Invoke(testEntity).Should().BeFalse();
            CreateLambdaFunction(_testInstance.ExcludeExpression("Tesd")).Invoke(testEntity).Should().BeFalse();
        }

        [Fact]
        public void Should_Provide_Correct_Expression_When_Calling_UseGermanSoundex()
        {
            _testInstance.UseGermanSoundex();
            _testInstance.IncludeAcceptsMultipleInputs.Should().BeTrue();

            var testEntity = new GenericSource { String = "Das ist ein Test" };

            CreateLambdaFunction(_testInstance.IncludeExpression("Das ist ein Test")).Invoke(testEntity).Should().BeTrue();
            CreateLambdaFunction(_testInstance.IncludeExpression("Test")).Invoke(testEntity).Should().BeTrue();
            CreateLambdaFunction(_testInstance.IncludeExpression("Täst")).Invoke(testEntity).Should().BeTrue();
            CreateLambdaFunction(_testInstance.IncludeExpression("Desd")).Invoke(testEntity).Should().BeTrue();

            CreateLambdaFunction(_testInstance.ExcludeExpression("Das ist ein Test")).Invoke(testEntity).Should().BeTrue();
            CreateLambdaFunction(_testInstance.ExcludeExpression("Test")).Invoke(testEntity).Should().BeTrue();
            CreateLambdaFunction(_testInstance.ExcludeExpression("Täst")).Invoke(testEntity).Should().BeFalse();
            CreateLambdaFunction(_testInstance.ExcludeExpression("Tesd")).Invoke(testEntity).Should().BeFalse();
        }

        [NotNull]
        private static Func<GenericSource, bool> CreateLambdaFunction(Expression searchExpression)
        {
            var body = searchExpression;
            var parameterExpression = Expression.Parameter(typeof(GenericSource));
            var rewrittenExpression = PredicateRewriter.Rewrite(parameterExpression, body);
            var lambda = Expression.Lambda<Func<GenericSource, bool>>(rewrittenExpression, parameterExpression);
            return lambda.Compile();
        }
    }
}
