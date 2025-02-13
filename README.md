# Commentator.OrchardCore
Internal Comment Engine Module for Orchard Core.

# Getting Started
- Install NuGet package 

  - ``dotnet add package Commentator.OrchardCore``
- Launch your Orchard Core application, login as admin, then go to the Features admin page and enable the Commentator module
- Add the Commentator Part to your Content type

# Liquid Template
You will need to add `` {{ Model.Content.CommentatorPart | shape_render }} `` to your template to display the commentator part. It will display by default if you are not using a custom template

# Commentator Options
- Order By - Used to sort comments by Date, Rank (not yet implemented)
- Group By - Used to display the comments by thread or single
- Editor - Type of editor to display for the comment. Currently supported CKEditor and Trumbowyg
- Page Count - # of Items to display per page. (Currently Pager is hidden as it needs to be implemented to use jQuery to display pages)

# Content Type Settings
- Allow Comments - This will toggle the comment window to show or not

# Pending Features
- Admin Dashboard to maintain Comments
- Pager on Comment List
- Handle unathorize user. Add a message or pop up

# New Features 1.2.0-rc-1
- Email Notifications - using 'Notification.OrchardCore' Nuget Library
