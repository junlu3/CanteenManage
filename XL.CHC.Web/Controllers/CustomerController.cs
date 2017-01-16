using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using XL.CHC.Domain.DomainModel;
using XL.CHC.Domain.Interfaces.Services;
using XL.CHC.Web.Models;

namespace XL.CHC.Web.Controllers
{
    public class CustomerController : BaseController
    {

        #region Fields
        private readonly IMSDS_CustomerService _customerService;
        #endregion

        public CustomerController(IMSDS_CustomerService customerService)
        {
            _customerService = customerService;
        }

        // GET: Customer
        public ActionResult Index()
        {
            try
            {
                var model = new CustomerSearchViewModel();
                SearchOrders(model);
                return View(model);
            }
            catch (Exception ex)
            {
                ErrorNotification(ex);
                return View();
            }
            
        }


        [HttpPost]
        public ActionResult Index(CustomerSearchViewModel searchModel)
        {
            try
            {
                SearchOrders(searchModel);
                return View(searchModel);
            }
            catch (Exception ex)
            {
                var model = new CustomerSearchViewModel();
                SearchOrders(model);
                ErrorNotification(ex);
                return View(model);
            }
        }



        private void SearchOrders(CustomerSearchViewModel model)
        {
            CustomerSearchModel searchModel = new CustomerSearchModel {
                PageIndex = model.PageIndex,
                PageSize = model.PageSize,
                KeyWord = model.Keyword
            };

            model.ViewList = _customerService.Search(searchModel);
           
        }


        public ActionResult CreateOrUpdate(Guid? id = null)
        {
            try
            {
                if (id != null)
                {
                    var entity = _customerService.Single(id.Value);
                    if (entity != null)
                    {
                        return View(entity);
                    }
                    else
                    {
                        ErrorNotification(new Exception("加载失败，未找到该员工"));
                        return RedirectToAction("Index");
                    }
                }
                else
                {
                    MSDS_Customer model = new MSDS_Customer();
                    return View(model);
                }
            }
            catch (Exception ex)
            {
                ErrorNotification(new Exception("员工编辑页面加载失败" + ex.Message));
                return RedirectToAction("Index");
            }
        }

        [HttpPost]
        public ActionResult CreateOrUpdate(MSDS_Customer model)
        {
            try
            {
                if (string.IsNullOrEmpty(model.EMPLOYEE_CARD))
                {
                    ModelState.AddModelError("EMPLOYEE_CARD", "饭卡号不能为空");
                }
                else if (string.IsNullOrEmpty(model.EMPLOYEE_ID))
                {
                    ModelState.AddModelError("EMPLOYEE_ID", "员工号不能为空");
                }
                var errors = ModelState.Values.SelectMany(v => v.Errors);
                if (ModelState.IsValid)
                {
                    if (model.ROW_ID.ToString() == "00000000-0000-0000-0000-000000000000")
                    {
                        using (var unitOfWork = UnitOfWorkManager.NewUnitOfWork())
                        {

                            model.ROW_ID = Guid.NewGuid();
                            model.EMPLOYEE_CARD = model.EMPLOYEE_CARD.Trim();
                            model.EMPLOYEE_ID = model.EMPLOYEE_ID.Trim();
                            model.EMPLOYEE_NAME = string.IsNullOrEmpty(model.EMPLOYEE_NAME) ? "" : model.EMPLOYEE_NAME.Trim();
                            model.EMPLOYEE_NAME_CN = string.IsNullOrEmpty(model.EMPLOYEE_NAME_CN) ? "" : model.EMPLOYEE_NAME_CN.Trim();
                            model.EMPLOYEE_NAME_EN = string.IsNullOrEmpty(model.EMPLOYEE_NAME_EN) ? "" : model.EMPLOYEE_NAME_EN.Trim();
                            model.COMPANY_CODE = string.IsNullOrEmpty(model.COMPANY_CODE) ? "" : model.COMPANY_CODE.Trim();
                            model.DEPARTMENT_NAME = string.IsNullOrEmpty(model.DEPARTMENT_NAME) ? "" : model.DEPARTMENT_NAME.Trim();
                            model.MGR_NAME = string.IsNullOrEmpty(model.MGR_NAME) ? "" : model.MGR_NAME.Trim();
                            model.LOCATION = string.IsNullOrEmpty(model.LOCATION) ? "" : model.LOCATION.Trim();
                            model.COMBO_CODE = string.IsNullOrEmpty(model.COMBO_CODE) ? "" : model.COMBO_CODE.Trim();
                            _customerService.Add(model);

                            unitOfWork.Commit();

                            SuccessNotification("添加成功");

                            return View(model);
                        }
                    }
                    else
                    {
                        var entity = _customerService.Single(model.ROW_ID);
                        var entity2 = _customerService.Single(model.EMPLOYEE_CARD);
                        if (entity != null)
                        {
                            using (var unitOfWork = UnitOfWorkManager.NewUnitOfWork())
                            {

                                if (entity2 != null && model.ROW_ID != entity2.ROW_ID)
                                {
                                    ErrorNotification(new Exception("饭卡号被占用"));
                                    return View(model);
                                }
                                else
                                {
                                    entity.EMPLOYEE_CARD = model.EMPLOYEE_CARD.Trim();
                                    entity.EMPLOYEE_ID = model.EMPLOYEE_ID.Trim();
                                    entity.EMPLOYEE_NAME = string.IsNullOrEmpty(model.EMPLOYEE_NAME)? "" : model.EMPLOYEE_NAME.Trim();
                                    entity.EMPLOYEE_NAME_CN = string.IsNullOrEmpty(model.EMPLOYEE_NAME_CN) ? "" : model.EMPLOYEE_NAME_CN.Trim();
                                    entity.EMPLOYEE_NAME_EN = string.IsNullOrEmpty(model.EMPLOYEE_NAME_EN) ? "" : model.EMPLOYEE_NAME_EN.Trim();
                                    entity.CARD_STATUS = model.CARD_STATUS;
                                    entity.COMPANY_CODE = string.IsNullOrEmpty(model.COMPANY_CODE) ? "" : model.COMPANY_CODE.Trim();
                                    entity.DEPARTMENT_NAME = string.IsNullOrEmpty(model.DEPARTMENT_NAME) ? "" : model.DEPARTMENT_NAME.Trim();
                                    entity.MGR_NAME = string.IsNullOrEmpty(model.MGR_NAME) ? "" : model.MGR_NAME.Trim();
                                    entity.LOCATION = string.IsNullOrEmpty(model.LOCATION) ? "" : model.LOCATION.Trim();
                                    entity.COMBO_CODE = string.IsNullOrEmpty(model.COMBO_CODE) ? "" : model.COMBO_CODE.Trim();

                                    entity.IS_BREAKFAST = model.IS_BREAKFAST;
                                    entity.IS_CHINESE_FOOD = model.IS_CHINESE_FOOD;
                                    entity.IS_WEST_FOOD = model.IS_WEST_FOOD;
                                    entity.IS_SPECIAL_FOOD = model.IS_SPECIAL_FOOD;
                                    entity.IS_COFFEE = model.IS_COFFEE;
                                    

                                    unitOfWork.Commit();
                                    SuccessNotification("编辑成功");
                                    return View(model);
                                }
                            }
                        }
                        else
                        {
                            ErrorNotification(new Exception("编辑失败，未找到该员工"));
                            return RedirectToAction("Index");
                        }
                    }
                }
                else
                {
                    ErrorNotification(new Exception("编辑失败，输入信息有误"));
                    return View(model);
                }
                
            }
            catch (Exception ex)
            {
                ErrorNotification(ex);
                return View(model);
            }
        }

        public ActionResult Delete(Guid id)
        {
            try
            {
                using (var unitOfWork = UnitOfWorkManager.NewUnitOfWork())
                {
                    var entity = _customerService.Single(id);
                    if (entity != null)
                    {
                        _customerService.Delete(entity);
                        unitOfWork.Commit();
                        SuccessNotification("删除成功");
                    }
                    else
                    {
                        ErrorNotification(new Exception("删除失败，未找到Id为" + id.ToString() + "的员工"));
                    }
                    return RedirectToAction("Index");
                }
            }
            catch (Exception ex)
            {
                ErrorNotification(ex);
                return RedirectToAction("Index");
            }

        }
    }
}