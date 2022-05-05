using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using NUnit.Framework;
using System;
using System.Threading.Tasks;
using Tidbits.EntityFramework.Exceptions;

namespace Tidbits.EntityFramework.Tests
{
    public class CrudDbContextTests
    {
#pragma warning disable CS8618
        private TestDbContext _context;
#pragma warning restore CS8618

        [SetUp]
        public void Setup()
        {
            var dbContextOptions = new DbContextOptionsBuilder<TestDbContext>()
                .UseInMemoryDatabase(databaseName: "testdb")
                .Options;

            _context = new TestDbContext(dbContextOptions);
        }

        [TearDown]
        public async Task TearDown()
        {
            await _context.Database.EnsureDeletedAsync();
        }

        [Test]
        public async Task AddNoTracking_clearsObjectFromChangeTracker_andReturnsNewEntity()
        {
            // Arrange
            await _context.AddRangeAsync(C.Users);
            await _context.SaveChangesAsync();

            // Act
            var result = await _context.AddNoTracking(new User() { Id = 6, Name = "blah" });

            // Assert
            _context.ExistsLocally(C.Users[0]).Should().BeTrue();
            result.Should().NotBeNull();
            _context.ExistsLocally(result).Should().BeFalse();
        }

        [Test]
        public async Task RemoveEntity_deletesAndReturnsEntity_whenExactlyOneMatchFound()
        {
            // Arrange
            await _context.AddRangeAsync(C.Users);
            await _context.SaveChangesAsync();

            // Act
            var result = await _context.Remove<User>(e => e.Id == C.Users[0].Id);

            // Assert
            var exists = await _context.Exists<User>(e => e.Id == C.Users[0].Id);
            result.Should().NotBeNull();
            result.Should().BeEquivalentTo(C.Users[0]);
            exists.Should().BeFalse();
        }

        [Test]
        public async Task RemoveEntity_doesNothing_whenNoMatchFound()
        {
            // Arrange
            await _context.AddRangeAsync(C.Users);
            await _context.SaveChangesAsync();

            // Act
            var result = await _context.Remove<User>(e => e.Id == 10);

            // Assert
            result.Should().BeNull();
        }

        [Test]
        public async Task RemoveEntity_shouldThrow_whenMoreMatchesFound()
        {
            // Arrange
            await _context.AddRangeAsync(C.Users);
            await _context.SaveChangesAsync();

            // Act
            var act = async () => await _context.Remove<User>(e => e.Id > 1);

            // Assert
            await act.Should().ThrowAsync<InvalidOperationException>().WithMessage("Sequence contains more than one element");
        }

        [Test]
        public async Task UpdateNoTracking_updatesCorrectProperties_whenFoundAndAlreadyTracked()
        {
            // Arrange
            await _context.AddRangeAsync(C.Users);
            await _context.SaveChangesAsync();
            var original = C.Users[0];
            var changed = new User()
            { Id = 1, Name = "other name", Property1 = 2, Property2 = 2, Property3 = 2, Property4 = 2 };

            // Act
            var affectedRows = await _context.UpdateNoTracking(changed);

            // Assert
            affectedRows.Should().Be(1);
            var updated = await _context.Users.SingleAsync(e => e.Id == changed.Id);
            updated.Should().NotBeNull();
            updated.Should().BeEquivalentTo(changed);
            original.Should().NotBeEquivalentTo(changed, options => options.Excluding(e => e.Id));
        }

        [Test]
        public async Task UpdateNoTracking_updatesCorrectProperties_whenFoundAndNotTracked()
        {
            // Arrange
            await _context.AddRangeAsync(C.Users);
            await _context.SaveChangesAsync();
            var original = C.Users[0];
            var changed = new User()
            { Id = 1, Name = "other name", Property1 = 2, Property2 = 2, Property3 = 2, Property4 = 2 };
            _context.ChangeTracker.Clear();

            // Act
            var affectedRows = await _context.UpdateNoTracking(changed);

            // Assert
            affectedRows.Should().Be(1);
            var updated = await _context.Users.SingleAsync(e => e.Id == changed.Id);
            updated.Should().NotBeNull();
            updated.Should().BeEquivalentTo(changed);
            original.Should().NotBeEquivalentTo(changed, options => options.Excluding(e => e.Id));
        }


        [Test]
        public async Task UpdateNoTracking_throwsException_whenNotFound()
        {
            // Arrange
            var changed = new User()
            { Id = 10, Name = "other name", Property1 = 2, Property2 = 2, Property3 = 2, Property4 = 2 };

            // Act
            var act = async () => await _context.UpdateNoTracking(changed);

            // Assert
            await act.Should()
                .ThrowAsync<DbNotFoundException>()
                .WithMessage("Entity not found in database");
        }

        [Test]
        public async Task UpdateNoTrackingByPropertyName_updatesCorrectProperties_whenFoundAndNotTracked()
        {
            // Arrange
            await _context.AddRangeAsync(C.Users);
            await _context.SaveChangesAsync();
            var original = C.Users[0];
            var changed = new User()
            { Id = 1, Name = "other name", Property1 = 2, Property2 = 2, Property3 = 2, Property4 = 2 };
            _context.ChangeTracker.Clear();

            // Act
            var affectedRows = await _context.UpdateNoTracking(changed, "Property1", "Property3");

            // Assert
            affectedRows.Should().Be(1);
            var updated = await _context.Users.SingleAsync(e => e.Id == changed.Id);
            updated.Should().NotBeNull();
            updated.Name.Should().Be(original.Name);
            updated.Property1.Should().Be(changed.Property1);
            updated.Property2.Should().Be(original.Property2);
            updated.Property3.Should().Be(changed.Property3);
            updated.Property4.Should().Be(original.Property4);
        }

        [Test]
        public async Task UpdateNoTrackingByPropertySelector_updatesCorrectProperties_whenFoundAndNotTracked()
        {
            // Arrange
            await _context.AddRangeAsync(C.Users);
            await _context.SaveChangesAsync();
            var original = C.Users[0];
            var changed = new User()
            { Id = 1, Name = "other name", Property1 = 2, Property2 = 2, Property3 = 2, Property4 = 2 };
            _context.ChangeTracker.Clear();

            // Act
            var affectedRows = await _context.UpdateNoTracking(changed, e => e.Property2, e => e.Property4);

            // Assert
            affectedRows.Should().Be(1);
            var updated = await _context.Users.SingleAsync(e => e.Id == changed.Id);
            updated.Should().NotBeNull();
            updated.Name.Should().Be(original.Name);
            updated.Property1.Should().Be(original.Property1);
            updated.Property2.Should().Be(changed.Property2);
            updated.Property3.Should().Be(original.Property3);
            updated.Property4.Should().Be(changed.Property4);
        }
    }
}
