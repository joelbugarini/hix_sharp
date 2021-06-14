# hix code generator
Flexible code generator that grows with you and your project. 

Some generators lock you in some way or another, in some cases generators don't fit your architectural needs, but with hix, you are in control. The engine takes a model and a template (both can be modified) to generate an output file. 

To understand hix, first we need to understand the parts that move it. 
* First we have the `templates`, that are text files with a specific syntax (more on this later) used by the engine.
* Then we have the `models`, that are just json files with the object definition.
* At last we have the `generator`, or hix as we called, the cli used to interact with the engine.

## templates

TODO: explanation

## motivation 

Long time ago, I was thrilled by a tool called Lightswitch. This tool made possible to create an app in mater of days, not kidding. Just define a class model and lightswitch created the Views (Create Page, List Page, Edit Page, etc.) with all the CRUD functionallity.

It was developed by Microsoft and hardly integrated in to visual studio, the tool had some heavy downsides and it was intended for .Net environments. But it was enough for some cases. Nowdays the tool has mutated/merged in to Microsoft Power Apps, and there is no other tool that works as lightswitch used to.

That's why I started this project, my intention was to create a tool that let me work like this, but if I needed to change something it also could.
