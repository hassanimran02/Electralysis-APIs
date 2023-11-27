using ArgaamPlus.Shared.DataServices;
using Common.DAL.Entities;
using Macroeconomics.Api.Controllers;
using Macroeconomics.BLS.Services.Common;
using Macroeconomics.BLS.Services.EconomicIndicatorFields;
using Macroeconomics.DAL.Entities;
using Macroeconomics.BLS.Models.EconomicIndicatorField;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Net.WebSockets;
using System.Text.RegularExpressions;
using static Azure.Core.HttpHeader;
using EconomicIndicatorField = Macroeconomics.DAL.Entities.EconomicIndicatorField;

namespace Macroeconomics.UnitTests
{
    public class EconomicIndicatorFieldUnitTest
    {
        IEconomicIndicatorFieldService _service;
        EconomicIndicatorFieldController _controller;
        ArgaamNext_IndicatorContext _mockContext;

        public EconomicIndicatorFieldUnitTest()
        {
            // Initialize a mock database context
            var dbContextOptions = new DbContextOptionsBuilder<ArgaamNext_IndicatorContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;
            _mockContext = new ArgaamNext_IndicatorContext(dbContextOptions);
            _mockContext.EconomicIndicatorGroups.Add(new DAL.Entities.EconomicIndicatorGroup() { GroupID = 22, NameEn = "", NameAr = "", ParentGroupID = 1, DisplaySeqNo = 97 });
            _mockContext.EconomicIndicatorFields.Add(new EconomicIndicatorField() { EconomicIndicatorFieldID = 21, DisplayNameEn = "Currency outside Banks", DisplayNameAr = "النقد المتداول خارج المصارف", DisplaySeqNo = 1, MeasuringUnitID = 4, GroupID = 22, IsChart = Convert.ToBoolean(0) });
            _mockContext.SaveChanges();
            var _genericService = new GenericRepository<EconomicIndicatorField, EconomicIndicatorFieldModel>(_mockContext);

            // Initialize the service with the mock context
            _service = new EconomicIndicatorFieldService(_genericService);
            // Initialize the controller with the service
            _controller = new EconomicIndicatorFieldController(_service);
        }

        [Fact]
        public async Task Get_EconomicIndicatorFieldById_Success()
        {
            short validFieldId = 21;
            var groupId = 22;
            var successResult = await _controller.GetAsync(groupId, validFieldId);
            var responseSuccess = successResult as OkObjectResult;
            Assert.NotNull(responseSuccess);
            if (responseSuccess.StatusCode == StatusCodes.Status200OK)
            {
                //Assert.Equal(StatusCodes.Status200OK, responseSuccess.StatusCode);
                var obj = responseSuccess.Value as EconomicIndicatorFieldModel;
                Assert.True(obj?.EconomicIndicatorFieldId > 0);
            }
            else
            {
                Assert.Fail();
            }

            //fail
            //short inValidFieldId = 22;
            //var errorResult = await _controller.GetAsync(groupId, inValidFieldId); // It breaks the code, due to the middleware
            //var responseError = errorResult as NotFoundResult;
            //Assert.IsType<NotFoundResult>(responseError.StatusCode == StatusCodes.Status404NotFound);
        }

        [Fact]
        public async Task Add_EconomicIndicatorField_Success()
        {
            EconomicIndicatorFieldModel economicIndicatorFieldModel = new EconomicIndicatorFieldModel()
            {
                EconomicIndicatorFieldId = 25,
                DisplayNameEn = "Currency Testing",
                DisplayNameAr = " خارج المصارف",
                DisplaySeqNo = 0,
                MeasuringUnitId = 4,
                GroupId = 22,
                IsChart = Convert.ToBoolean(0)
            };

            int groupId = 22; // we can pass the groupId from economicIndicatorFieldModel instead of the creating groupId
            var successResult = await _controller.Save(groupId, economicIndicatorFieldModel);
            var responseSuccess = successResult as CreatedResult;
            Assert.NotNull(responseSuccess);
            if (responseSuccess?.StatusCode == StatusCodes.Status201Created)
            {
                var id = (Int16)responseSuccess?.Value; // Cant cast int16 to Int32
                Assert.True(id == 25);
                Assert.True(id > 0);
            }
            else
            {
                Assert.Fail();
            }
            // fail
        }

        [Fact]
        public async Task Update_EconomicIndicatorField_Success()
        {
            EconomicIndicatorFieldModel economicIndicatorFieldModel = new EconomicIndicatorFieldModel()
            {
                EconomicIndicatorFieldId = 21,
                DisplayNameEn = "Testing",
                DisplayNameAr = " خارج ",
                DisplaySeqNo = 0,
                MeasuringUnitId = 5,
                GroupId = 22,
                IsChart = Convert.ToBoolean(0)
            };

            var successResult = await _controller.Update(economicIndicatorFieldModel.GroupId, (economicIndicatorFieldModel.EconomicIndicatorFieldId), economicIndicatorFieldModel);
           
            var responseSuccess = successResult as NoContentResult;
            Assert.NotNull(responseSuccess);
            Assert.True(responseSuccess.StatusCode == 204);
        }
       
        [Fact]
        public async Task Delete_EconomicIndicatorField_Success()
        {
            int EconomicIndicatorFieldId = 21;
            int groupId = 22;
            var successResult = await _controller.Delete(groupId, EconomicIndicatorFieldId);
            var responseSuccess = successResult as NoContentResult;
            Assert.NotNull(responseSuccess);
            Assert.True(responseSuccess.StatusCode == StatusCodes.Status204NoContent);
        }

        [Fact]
        public async Task GetAll_EconomicIndicatorField_Success()
        {
            int groupId = 22;
            var successResult = await _controller.GetAsync(groupId);
            
            var responseSuccess = successResult as OkObjectResult;
            Assert.NotNull(responseSuccess);
            Assert.True(responseSuccess.StatusCode == StatusCodes.Status200OK);

            var EconomicIndicatorFieldData = responseSuccess.Value as List<EconomicIndicatorFieldModel>;
            Assert.True(EconomicIndicatorFieldData?.Count == 1);

            if (EconomicIndicatorFieldData.Count > 1)
            {
                Assert.Fail();
            }

        }

    }
}