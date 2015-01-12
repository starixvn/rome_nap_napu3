// --------------------------------------------------------------------------
// <copyright file="GetAdminGroupCriteria.cs" company="Nexcel Solutions Vietnam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------
// <history>
//   <change who="Paul.Nguyen" date="2015.01.07">Created</change>
// </history>

namespace Fanex.Tarus.User.Criteria
{
    using Fanex.NAP.Framework.Data;

    /// <summary>
    /// The class of GetAdminGroupCriteria.
    /// </summary>
    public class GetAdminGroupCriteria : CriteriaBase
    {
        public string UserName { get; set; }

        public override bool IsValid()
        {
            return true;
        }

        public override string GetSettingKey()
        {
            return "Acc_AdminGroupSel";
        }
    }
}
