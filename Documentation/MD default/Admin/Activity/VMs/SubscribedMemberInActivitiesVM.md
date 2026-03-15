# Subscribed Members in Activity

## ViewModel Details
**Model Name:** `SubscribedMemberInActivitiesVM`  
**Namespace:** `FougeraClub.Areas.Admin.ViewModels.Activity`

Used to manage and display **paginated lists of members** who are subscribed to a specific activity.

---

## Structure

| Property | Description | Notes |
|---------|-------------|-------|
| **Activity** | The selected activity details | Uses `ActivityVM` |
| **Members_Paginated** | Paginated list of members subscribed to the activity | Uses `PaginatedList<MemberVM>` for paging UI support |
| **Subscriptions** | Raw subscription records linked to the activity | Uses `IEnumerable<Subscription>` |

---

## Usage Context
- Admin → Activities → View / Manage Subscribed Members  
- Provides structured pagination for large lists  
- Enables linking between **Activity**, **Members**, and **Subscription** records
