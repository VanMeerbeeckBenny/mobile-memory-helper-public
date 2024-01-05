
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Mde.Project.Mobile.Domain.Models;
using Mde.Project.Mobile.Domain.Services.Api;
using Mde.Project.Mobile.Domain.Services.Interfaces;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace Mde.Project.Mobile.UWPTests
{
    [TestClass]
    public class FirebaseListServiceTest
    {       

        [TestMethod]
        public async Task CreateList_WithNull_ReturnsIsSuccesFalse()
        {
            //Arrange
            var _listService = new FirebaseListService();
            ListModel listToAdd = null;

            //Act
            var result = await _listService.CreateList(listToAdd);

            //Assert
            Assert.IsFalse(result.IsSucces);
        }

        [TestMethod]
        public async Task CreateList_WithValidList_ReturnsIsSuccesTrue()
        {
            //Arrange
            var _listService = new FirebaseListService();
            ListModel listToAdd = new ListModel { Id = "5",IsShared = false,Name = "CoolNewAwsomeList",OwnerId = "Zf1dO8dX2mX8hqXMfPMlR1BPEmj2" };

            //Act
            var result = await _listService.CreateList(listToAdd);

            //Assert
            Assert.IsTrue(result.IsSucces);
        }

        [TestMethod]
        public async Task GetById_WithValidId_ReturnsIsSuccesTrue()
        {
            //Arrange
            var _listService = new FirebaseListService();
            string id = "2b5c5594-36ff-4b2a-b446-6a86e0de412b";

            //Act
            var result = await _listService.GetListById(id);
            var foundItem = result.Items.FirstOrDefault();

            //Assert
            Assert.IsTrue(result.IsSucces);
            Assert.AreEqual(id, foundItem.Id);
        }
        [TestMethod]
        public async Task DeleteList_WithValidId_ReturnsIsSuccesFalse()
        {
            //Arrange
            var _listService = new FirebaseListService();

            //Act
            var result = await _listService.DeleteList("5");

            //Assert
            Assert.IsTrue(result.IsSucces);
        }

        [TestMethod]
        public async Task GetById_WithInvalidId_ReturnsIsSuccesFalse()
        {
            //Arrange
            var _listService = new FirebaseListService();           

            //Act
            var result = await _listService.GetListById("6");

            //Assert
            Assert.IsFalse(result.IsSucces);
        }


        [TestMethod]
        public async Task GetListByUserId_WithValidID_ReturnItems()
        {
            //Arrange
            var _listService = new FirebaseListService();
            string id = "Zf1dO8dX2mX8hqXMfPMlR1BPEmj2";
       
            //Act
            var result = await _listService.GetListbyUserId(id);

            //Assert            
            Assert.IsTrue(result.Items.Any());
            Assert.IsTrue(result.Items.All(i => i.OwnerId == id));
        }

        [TestMethod]
        public async Task GetListByUserId_WithInValidID_ReturnsNull()
        {
            //Arrange
            var _listService = new FirebaseListService();
            string id = "5";

            //Act
            var result = await _listService.GetListbyUserId(id);

            //Assert            
            Assert.IsFalse(result.IsSucces);
            Assert.IsNull(result.Items);
        }
    }
}
