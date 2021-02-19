﻿using Common;
using MISA.Common;
using MISA.DataLayer.Interfaces;
using MISA.Service.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace MISA.Service
{
    public class EmployeeeService :BaseService<Employeee>,IEmployeeeService
    {
        #region CONTRUCTOR
        public EmployeeeService(IEmployeeeRepository<Employeee> employeeeRepository) : base(employeeeRepository)
        {

        }
        #endregion

        #region METHODS
        /// <summary>
        /// Lấy toàn bộ dữ liệu 
        /// </summary>
        /// <returns>toàn bộ dữ liệu</returns>
        /// CreatedBy: TLMinh (18/02/2021)
        public override ServiceResult Get()
        {
            var serviceResult = new ServiceResult();
            serviceResult.Data = _dbContext.GetAll("Select * from Employeee Order By EmployeeeCode desc");
            return serviceResult;
        }

        /// <summary>
        /// Validate dữ liệu nhân viên
        /// </summary>
        /// <param name="employeee">Thực thể nhân viên cần validate dữ liệu</param>
        /// <param name="errorMessenger">Tập hợp các thông báo lỗi</param>
        /// <param name="entityCode">Mã nhân viên tương ứng</param>
        /// <returns>True: dữ liệu hợp lệ; False: dữ liệu không hợp lệ</returns>
        /// CreatedBy : TLMinh (18/02/2021)
        public override bool Validate(Employeee employeee, ErrorMessenger errorMessenger, string entityCode = null)
        {
            var isValid = true;

            //Validate dữ liệu
            //Nếu là update thông tin thì kiểm tra xem mã thực thể có thay đổi không 
            //Nếu thay đổi thì phải check trùng mã,check trùng chứng minh thư nhân dân
            //Nếu không thì không cần check
            if (entityCode == null || entityCode != employeee.EmployeeeCode)
            {
                //Kiểm tra trùng mã nhân viên
                List<Employeee> employeeeCodeExist = (List<Employeee>)_dbContext.GetAll($"SELECT * FROM Employeee Where EmployeeeCode = '{employeee.EmployeeeCode}'");
                if (employeeeCodeExist.Count != 0)
                {
                    errorMessenger.UserMsg.Add(MISA.Common.Properties.Resources.ErrorService_DuplicateEmployeeCode);
                    isValid = false;
                }

                //Kiểm tra trùng số chứng minh thư nhân dân
                List<Employeee> employeeeIdentityExist = (List<Employeee>)_dbContext.GetAll($"SELECT * FROM Employeee Where Identity = '{employeee.Identity}'");
                if (employeeeIdentityExist.Count != 0)
                {
                    errorMessenger.UserMsg.Add(MISA.Common.Properties.Resources.ErrorService_DuplicateEmployeeIdentity);
                    isValid = false;
                }
            }

            //Bắt buộc nhập những thông tin(Mã Nhân Viên, Email, Họ Và Tên, Số Điện Thoại, Chứng Minh Thư Nhân Dân)
            if (employeee.EmployeeeCode == string.Empty || employeee.FullName == string.Empty 
                || employeee.Identity == string.Empty || employeee.Identity == null
                || employeee.EmployeeeCode == null || employeee.FullName == null)
            {
                isValid = false;
                errorMessenger.UserMsg.Add(MISA.Common.Properties.Resources.ErrorService_EmptyDataInput);
            }

            return isValid;
        }

        #endregion 
    }
}