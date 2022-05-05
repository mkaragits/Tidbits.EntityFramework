using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;
using FluentAssertions;
using Moq;
using Moq.EntityFrameworkCore;
using NUnit.Framework;

namespace Tidbits.EntityFramework.Tests
{
    public class ReadOnlyDbContextTests
    {
        private IReadOnlyDbContext _sut;

        [SetUp]
        public void Setup()
        {
            var ctxMock = new Mock<TestDbContext>();
            ctxMock.SetupSequence(x => x.Set<User>())
                .ReturnsDbSet(C.Users);
            _sut = ctxMock.Object;
        }

        [Test]
        public async Task ListAllNoTracking_returns_fullList()
        {
            // Act
            var seq = _sut.ListAllNoTracking<User>();
            var result = await seq.ToListAsync();

            // Assert
            result.Should().BeEquivalentTo(C.Users);
        }

        [Test]
        public async Task ListAllNoTracking_returns_fullProjectedList()
        {
            // Act
            var seq = _sut.ListAllNoTracking<User, ProjectedUser>(u => new ProjectedUser() { UserId = u.Id });
            var result = await seq.ToListAsync();

            // Assert
            result.Should().BeEquivalentTo(C.Users.Select(u=> new ProjectedUser(){UserId = u.Id}));
        }

        [Test]
        public async Task ListNoTracking_returns_correctElements()
        {
            // Act
            var seq = _sut.ListNoTracking<User>(u=>u.Name.StartsWith("J"));
            var result = await seq.ToListAsync();

            // Assert
            result.Should().BeEquivalentTo(new User[] {C.Users[0], C.Users[1]});
        }

        [Test]
        public async Task ListNoTracking_returns_correctProjectedElements()
        {
            // Arrange
            var expectedResult =
                (new User[] {C.Users[0], C.Users[1]})
                .Select(ProjectedUser.FromUser.Compile());

            // Act
            var seq = _sut
                .ListNoTracking<User, ProjectedUser>(
                u => u.Name.StartsWith("J"),
                ProjectedUser.FromUser);
            var result = await seq.ToListAsync();

            // Assert
            result.Should().BeEquivalentTo(expectedResult);
        }

        [Test]
        public async Task GetNoTracking_returns_correctElement()
        {
            // Act
            var result = await _sut.GetNoTracking<User>(u => u.Id == 1);

            // Assert
            result.Should().BeEquivalentTo(C.Users[0]);
        }

        [Test]
        public async Task GetNoTracking_returns_correctProjectedElement()
        {
            // Arrange
            var expectedResult = ProjectedUser.FromUser.Compile().Invoke(C.Users[0]);

            // Act
            var result = await _sut
                .GetNoTracking<User, ProjectedUser>(u => u.Id == 1, ProjectedUser.FromUser);


            // Assert
            result.Should().BeEquivalentTo(expectedResult);
        }

        [Test]
        public async Task GetNoTracking_shouldThrow_whenMoreThanOneMatchIsFound()
        {
            // Act
            var act = async () => await _sut.GetNoTracking<User>(u => u.Id > 1);

            // Assert
            await act.Should()
                .ThrowAsync<InvalidOperationException>()
                .WithMessage("Sequence contains more than one matching element");
        }

        [Test]
        public async Task GetNoTracking_should_returnDefaultValueIfNotFound()
        {
            // Act
            var result = await _sut.GetNoTracking<User>(u => u.Id == -1);

            // Assert
            result.Should().BeEquivalentTo(default(User));
        }

        [Test]
        public async Task Exists_returnsTrue_whenFound()
        {
            // Act
            var result = await _sut.Exists<User>(u => u.Id > 1);

            // Assert
            result.Should().BeTrue();
        }

        [Test]
        public async Task Exists_returnsFalse_whenNotFound()
        {
            // Act
            var result = await _sut.Exists<User>(u => u.Id == -1);

            // Assert
            result.Should().BeFalse();
        }
    }
}