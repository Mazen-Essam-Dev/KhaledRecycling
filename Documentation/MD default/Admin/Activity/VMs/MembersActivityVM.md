# Activity Members Overview

## ViewModel Details
**Model Name:** `MembersActivityVM`  
**Namespace:** `FougeraClub.Areas.Admin.ViewModels.Activity`

This ViewModel is used to display an activity along with its related members and their subscription status.

---

## Structure & Purpose

| Property | Description | Notes |
|---------|-------------|-------|
| **Activity** | Holds the activity information | Uses `ActivityVM` |
| **Members** | List of members related to the activity | Uses `MemberVM` |
| **Subscriptions** | List of member subscriptions to this activity | Uses `Subscription` entity |

---

## Usage Context
- Admin → Activities → View Participants
- Shows who has joined or is eligible to join the selected activity
- Used for displaying member participation lists and subscription management
