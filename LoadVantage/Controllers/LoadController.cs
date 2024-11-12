using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

using LoadVantage.Common.Enums;
using LoadVantage.Core.Contracts;
using LoadVantage.Core.Models.Load;
using LoadVantage.Extensions;
using LoadVantage.Filters;
using static LoadVantage.Common.GeneralConstants.ErrorMessages;
using static LoadVantage.Common.GeneralConstants.SuccessMessages;
using static LoadVantage.Common.GeneralConstants.ActiveTabs;

namespace LoadVantage.Controllers
{
    [Authorize]
    [Route("[controller]/[action]")]
	public class LoadController(ILoadStatusService loadService) : Controller
    {

        [HttpGet]
        public async Task<IActionResult> LoadDetails(Guid loadId)
        {

            Guid? userId = User.GetUserId();

            try
            {
                var loadToShow = await loadService.GetLoadDetailsAsync(loadId);

                if (loadToShow == null)
                {
	                return NotFound("The load you are looking for does not exist");
                }

                if (userId != loadToShow.BrokerId)
                {
					return RedirectToAction("LoadBoard", "LoadBoard"); // If the load does not belong to the broker redirect him back to the LoadBoard
				}

                return View(loadToShow);
            }
            catch (Exception e)
            {
                TempData.SetErrorMessage(ErrorRetrievingDetailsForLoad + " " + e.Message);
                return RedirectToAction("LoadDetails", new { loadId });
            }
        }

        [HttpGet]
        [BrokerOnly]
        public IActionResult CreateLoad()
        {
	        Guid? userId = User.GetUserId();
			var model = new LoadViewModel
            {
                BrokerId = userId.Value
			};

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [BrokerOnly]

        public async Task<IActionResult> CreateLoad(LoadViewModel model)
        {
            model.Status = LoadStatus.Created.ToString();

            Guid? userId = User.GetUserId();


            if (!ModelState.IsValid)
            {
                return View(model);
            }

            if (userId != model.BrokerId)
            {
				return RedirectToAction("LoadBoard", "LoadBoard"); // If the brokerId is not the same as logged user's id, redirect him back to the LoadBoard

			}

            try
            {
                var loadId = await loadService.CreateLoadAsync(model, model.BrokerId);

                TempData.SetSuccessMessage(LoadCreatedSuccessfully);

                return RedirectToAction("LoadDetails",  new { loadId });
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, ErrorCreatingLoad + ex.Message);
                return View("LoadDetails", model);
            }

        }

        [HttpPost]
        [ValidateAntiForgeryToken]

        public async Task<IActionResult> EditLoad(LoadViewModel model, bool isEditing, Guid loadId)
        {

            Guid? userId = User.GetUserId();

            if (isEditing)
            {
                if (!ModelState.IsValid)
                {
                    return View("LoadDetails", model);
                }

                try
                {
                    await loadService.EditLoadAsync(loadId, model);

                    if (userId != model.BrokerId)
                    {
						return RedirectToAction("LoadBoard", "LoadBoard"); // If the brokerId is not the same as logged user's id, redirect him back to the LoadBoard

					}

                    TempData["isEditing"] = false;
                    TempData.SetSuccessMessage(LoadUpdatedSuccessfully);

                }
                catch (Exception e)
                {
                    ModelState.AddModelError(string.Empty, ErrorUpdatingLoad + e.Message);
                    TempData.SetErrorMessage(ErrorUpdatingLoad + e.Message);
                }

                return RedirectToAction("LoadDetails", new { loadId = model.Id });
            }

            return RedirectToAction("LoadDetails", new { loadId = model.Id });

        }

        [HttpPost]
        [ValidateAntiForgeryToken]

        public async Task<IActionResult> CancelLoad(Guid loadId)
        {
            TempData["isEditing"] = false;

            Guid? userId = User.GetUserId();

            if (loadId == Guid.Empty)
            {
                TempData.SetErrorMessage(LoadCouldNotBeRetrieved + " " + LoadIdInvalid);
                return View("LoadDetails");
            }

            LoadViewModel load = await loadService.GetLoadByIdAsync(loadId);

            try
            {
                if (userId != load.BrokerId)
                {
					return RedirectToAction("LoadBoard", "LoadBoard"); // If the load's broker is not the same as logged user, redirect him back to the LoadBoard
				}

                await loadService.CancelLoadAsync(loadId);

                TempData.SetSuccessMessage(LoadCancelledSuccessfully);
				return RedirectToAction("LoadBoard", "LoadBoard");
			}
            catch (Exception e)
            {
                TempData.SetErrorMessage(ErrorCancellingLoad + e.Message);
                return View("LoadDetails", load);
            }

        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> PostALoad(Guid loadId)
        {
            Guid? userId = User.GetUserId();

            if (!ModelState.IsValid)
            {
                return RedirectToAction("LoadDetails", new { loadId });
            }

            if (loadId == Guid.Empty)
            {
                TempData.SetErrorMessage(LoadIdInvalid);
				return RedirectToAction("LoadBoard", "LoadBoard");
			}

            LoadViewModel load = await loadService.GetLoadByIdAsync(loadId);

            if (userId != load.BrokerId)
            {
				return RedirectToAction("LoadBoard", "LoadBoard"); // If the load's broker is not the same as logged user, redirect him back to the LoadBoard
			}

            if (load.Status != LoadStatus.Created.ToString())
            {
                if (load.Status == LoadStatus.Available.ToString())
                {
                    TempData.SetErrorMessage(LoadIsAlreadyPosted);
                }
                else
                {
                    TempData.SetErrorMessage(LoadIsNotInCorrectStatus);
                }

                return RedirectToAction("LoadDetails", new { loadId = load.Id });
            }

            try
            {
                await loadService.PostLoadAsync(load.Id);

                TempData.SetActiveTab(PostedActiveTab); // navigate to the posted tab
                TempData.SetSuccessMessage(LoadPostedSuccessfully);

                return RedirectToAction("LoadBoard", "LoadBoard");
			}
            catch (Exception e)
            {
                TempData.SetErrorMessage(ErrorPostingLoad + e.Message);
                return RedirectToAction("LoadDetails", new { loadId = load.Id });
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UnpostALoad(Guid loadId)
        {
            Guid? userId = User.GetUserId();

            if (!ModelState.IsValid)
            {
                return RedirectToAction("LoadDetails", new { loadId });
            }

            if (loadId == Guid.Empty)
            {
                TempData.SetErrorMessage(LoadIdInvalid);
                return RedirectToAction("LoadBoard", "LoadBoard");
			}

            LoadViewModel load = await loadService.GetLoadByIdAsync(loadId);


            if (userId != load.BrokerId)
            {
				return RedirectToAction("LoadBoard", "LoadBoard"); // If the load's broker is not the same as logged user, redirect him back to the LoadBoard
			}

            if (load.Status != LoadStatus.Available.ToString())
            {
                if (load.Status == LoadStatus.Created.ToString())
                {
                    TempData.SetErrorMessage(LoadIsNotPosted); // if the load is already not posted ( created status ) 
                }
                else
                {
                    TempData.SetErrorMessage(LoadIsNotInCorrectStatus); // if the load is booked or billed 
                }

                return RedirectToAction("LoadDetails", new { loadId = load.Id });
            }

            try
            {
                await loadService.UnpostLoadAsync(load.Id); // unpost the load

                TempData.SetActiveTab(CreatedActiveTab); // navigate to the created tab
                TempData.SetSuccessMessage(LoadUnpostedSuccessfully);

				return RedirectToAction("LoadBoard", "LoadBoard");
			}
            catch (Exception e)
            {
                TempData.SetErrorMessage(ErrorUnpostingLoad + e.Message);
                return RedirectToAction("LoadDetails", new { loadId = load.Id });
            }
        }
    }
}
