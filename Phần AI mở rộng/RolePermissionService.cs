using System;
using System.Collections.Generic;

namespace HotelManagerWpf.PhanAiMoRong
{
    public enum AppFeature
    {
        Dashboard,
        Rooms,
        CustomerRegistration,
        CustomerDetails,
        CheckOut,
        Employees,
        UserStatus
    }

    public enum PermissionAction
    {
        View,
        Create,
        Update,
        Delete
    }

    public static class RolePermissionService
    {
        private static readonly Dictionary<string, HashSet<AppFeature>> Permissions = new(StringComparer.OrdinalIgnoreCase)
        {
            [UserRole.Admin] =
            [
                AppFeature.Dashboard,
                AppFeature.Rooms,
                AppFeature.CustomerRegistration,
                AppFeature.CustomerDetails,
                AppFeature.CheckOut,
                AppFeature.Employees,
                AppFeature.UserStatus
            ],
            [UserRole.Manager] =
            [
                AppFeature.Dashboard,
                AppFeature.Rooms,
                AppFeature.CustomerRegistration,
                AppFeature.CustomerDetails,
                AppFeature.CheckOut,
                AppFeature.UserStatus
            ],
            [UserRole.Staff] =
            [
                AppFeature.Dashboard,
                AppFeature.CustomerRegistration,
                AppFeature.CustomerDetails,
                AppFeature.CheckOut
            ]
        };

        public static bool CanAccess(string? role, AppFeature feature)
        {
            var normalizedRole = NormalizeRole(role);
            return Permissions.TryGetValue(normalizedRole, out var allowedFeatures) && allowedFeatures.Contains(feature);
        }

        public static bool CanUseAction(string? role, AppFeature feature, PermissionAction action)
        {
            var normalizedRole = NormalizeRole(role);

            if (normalizedRole == UserRole.Admin) return true;
            if (!CanAccess(normalizedRole, feature)) return false;
            if (action == PermissionAction.Delete) return false;

            return normalizedRole switch
            {
                UserRole.Manager => feature != AppFeature.Employees,
                UserRole.Staff => feature is AppFeature.CustomerRegistration or AppFeature.CustomerDetails or AppFeature.CheckOut or AppFeature.Dashboard,
                _ => false
            };
        }

        public static string[] GetVisibleSubordinateRoles(string? role)
        {
            return NormalizeRole(role) switch
            {
                UserRole.Admin => [UserRole.Manager, UserRole.Staff, UserRole.Receptionist],
                UserRole.Manager => [UserRole.Staff, UserRole.Receptionist],
                _ => []
            };
        }

        public static string NormalizeRole(string? role)
        {
            if (string.Equals(role, UserRole.Receptionist, StringComparison.OrdinalIgnoreCase))
            {
                return UserRole.Staff;
            }

            return string.IsNullOrWhiteSpace(role) ? UserRole.Staff : role.Trim();
        }
    }
}
