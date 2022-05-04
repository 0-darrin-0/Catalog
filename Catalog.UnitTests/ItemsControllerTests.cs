using System.Reflection.Metadata.Ecma335;
using System.Runtime.Intrinsics.X86;
using System;
using System.Threading.Tasks;
using Catalog.Api.Controllers;
using Catalog.Api.DTOs;
using Catalog.Api.Entities;
using Catalog.Api.Repositories;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace Catalog.UnitTests
{
    public class ItemsControllerTests
    {

        private readonly Mock<IItemsRepository> repositoryStub = new();
        private readonly Mock<ILogger> loggerStub = new();

        private readonly Random rand = new();

        [Fact]
        public async Task GetItemAsync_ItemDoesNotExist_ReturnsNotFound()
        {
            var repositoryStub = new Mock<IItemsRepository>();

            repositoryStub.Setup(repo => repo.GetItemAsync(It.IsAny<Guid>()))
                .ReturnsAsync((Item)null);

                var loggerStub = new Mock<ILogger<ItemsController>>();

                var controller = new ItemsController(repositoryStub.Object, loggerStub.Object);

            var result = await controller.GetItemAsync(Guid.NewGuid()); 

            result.Result.Should().BeOfType<NotFoundResult>();

        }

        [Fact]
        public async Task GetItemAsync_ItemExists_ReturnsItem()
            {
                var expectedItem = CreateRandomItem();

                repositoryStub.Setup(repo => repo.GetItemAsync(It.IsAny<Guid>()))
                    .ReturnsAsync(expectedItem);

                var loggerStub = new Mock<ILogger<ItemsController>>();
                var controller = new ItemsController(repositoryStub.Object, loggerStub.Object);

                var result = await controller.GetItemAsync(Guid.NewGuid());

                result.Value.Should().BeEquivalentTo(
                    expectedItem,
                    options => options.ComparingByMembers<Item>());
            }

        [Fact]
        public async Task GetItemsAsync_ItemsExist_ReturnsItems()
            {
                var expectedItems = new[]{CreateRandomItem(),CreateRandomItem(),CreateRandomItem()};

                repositoryStub.Setup(repo => repo.GetItemsAsync())
                    .ReturnsAsync(expectedItems);

                var loggerStub = new Mock<ILogger<ItemsController>>();
                var controller = new ItemsController(repositoryStub.Object, loggerStub.Object);

                var actualItems = await controller.GetItemsAsync();

                actualItems.Should().BeEquivalentTo(
                    expectedItems,
                    options => options.ComparingByMembers<Item>()
                    );
            }

        private Item CreateRandomItem()
        {
            return new()
            {
                Id = Guid.NewGuid(),
                Name = Guid.NewGuid().ToString(),
                Price = rand.Next(1000),
                CreatedDate = DateTimeOffset.UtcNow
            };
        }

    };
}
