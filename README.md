# OrchardCore.Commentator
Internal Comment Engine Module for Orchard Core.

# Add Commentator to Content Type
The commentator part can be added to any content type

# Liquid Template
You will need to add {{ Model.Content.CommentatorPart | shape_render }} to your template to display the commentator part. It will display by default if you are not using a custom template

# Commentator Options
- Order By - Used to sort comments by Date, Rank (not yet implemented)
- Group By - Used to display the comments by thread or single
- Editor - Type of editor to display for the comment. Currently supported CKEditor and Trumbowyg
- Page Count - # of Items to display per page. (Currently Pager is hidden as it needs to be implemented to use jQuery to display pages)

# Content Type Settings
- Allow Comments - This will toggle the comment window to show or not
