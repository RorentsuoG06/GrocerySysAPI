using GrocerySysAppService;
using GrocerySysModels;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace GrocerySysAPI.Controllers
{
    [Route("api/items")]
    [ApiController]
    public class ItemController : ControllerBase
    {
        private readonly GroceryAppService _appService;

        public ItemController()
        {
            _appService = new GroceryAppService();
        }

        [HttpGet]
        public ActionResult<IEnumerable<Items>> GetAllItems()
        {
            var items = _appService.GetItems();
            return Ok(items);
        }

        [HttpGet("{id}", Name = "GetItemRoute")]
        public ActionResult<Items> GetItemByID(string id)
        {
            var account = _appService.FindItem(id);

            if (account == null)
            {
                return NotFound();
            }

            return Ok(account);
        }

        [HttpPost]
        public IActionResult CreateItem([FromBody] Models.ItemViewModel itemDto)
        {
            if (itemDto == null)
            {
                return BadRequest("Item data is required.");
            }

            var newItemData = new GrocerySysModels.Items
            {
                ItemId = "",
                ItemName = itemDto.ItemName ?? "Unnamed Item", 
                ItemLocation = itemDto.ItemLocation ?? "Unknown Location",
                ItemQuantity = itemDto.ItemQuantity ?? 0,
                CostPrice = itemDto.CostPrice ?? 0.00m,
                SellingPrice = itemDto.SellingPrice ?? 0.00m,
                WeightValue = itemDto.WeightValue ?? 0.0,
                Department = (GrocerySysModels.ProductDepartment)(itemDto.Department ?? 0),
                Unit = (GrocerySysModels.MeasurementUnit)(itemDto.Unit ?? 0),

                ExpirationDate = itemDto.ExpirationDate
            };

            _appService.addItems(newItemData);

            return CreatedAtRoute(
                "GetItemRoute",
                new { id = newItemData.ItemId },
                newItemData);
        }

        [HttpPatch("{id}")]
        public IActionResult UpdateItem(string id, [FromBody] Models.ItemViewModel itemDto)
        {
            if (itemDto == null)
            {
                return BadRequest("Update data is required.");
            }

            var existingItem = _appService.FindItem(id);
            if (existingItem == null)
            {
                return NotFound(new { Message = $"Cannot update. Item with ID '{id}' not found." });
            }

            if (!string.IsNullOrEmpty(itemDto.ItemName))
            {
                _appService.UpdateItemName(id, itemDto.ItemName);
            }

            if (itemDto.ItemQuantity.HasValue && itemDto.ItemQuantity >= 0)
            {
                _appService.UpdateItemQuantity(id, itemDto.ItemQuantity.Value);
            }

            if (!string.IsNullOrEmpty(itemDto.ItemLocation))
            {
                _appService.UpdateItemLocation(id, itemDto.ItemLocation);
            }

            if (itemDto.CostPrice.HasValue && itemDto.CostPrice > 0)
            {
                _appService.UpdateItemCostPrice(id, itemDto.CostPrice.Value);
            }

            if (itemDto.SellingPrice.HasValue && itemDto.SellingPrice > 0)
            {
                _appService.UpdateItemSellingPrice(id, itemDto.SellingPrice.Value);
            }

            if (itemDto.WeightValue.HasValue && itemDto.WeightValue > 0)
            {
                _appService.UpdateItemWeightValue(id, itemDto.WeightValue.Value);
            }

            if (itemDto.Department.HasValue)
            {
                _appService.UpdateItemDepartment(id, (GrocerySysModels.ProductDepartment)itemDto.Department.Value);
            }

            if (itemDto.Unit.HasValue)
            {
                _appService.UpdateItemUnit(id, (GrocerySysModels.MeasurementUnit)itemDto.Unit.Value);
            }

            if (itemDto.ExpirationDate.HasValue)
            {
                _appService.UpdateItemExpirationDate(id, itemDto.ExpirationDate);
            }

            return NoContent();
        }

        [HttpDelete("{id}")]
        public IActionResult DeleteItem(string id)
        {
            bool successfullyDeleted = _appService.DeleteItem(id);

           
            if (!successfullyDeleted)
            {
                return NotFound(new { Message = $"Cannot delete. Item with ID '{id}' does not exist." }); // HTTP 404
            }

            return NoContent(); // HTTP 204
        }
    }
}