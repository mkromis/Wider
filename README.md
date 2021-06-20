# Wider
This project is archived. No further updates are planned for this project.
Please contact me if you want to take over this project.

## Status Badge
[![Build Status](https://dev.azure.com/TorisanKitsune/Wider/_apis/build/status/TorisanKitsune.Wider)](https://dev.azure.com/TorisanKitsune/Wider/_build/latest?definitionId=1)

## Prism 7 changes.
Starting with Prism 7, the WiderBootstrapper is obsolete. 
This will require updating to WiderApplication that is the new style for Prism 7.
This also changes from Autofac to DryIoc. 

* See the updated tutorial in the wiki for more information. 

### What is this?
Wide is a participatory IDE framework which uses WPF and Prism to build IDE-like applications (eg: Visual Studio 2012/2013, Eclipse etc.) 

![Screenshot](https://raw.github.com/wiki/chandramouleswaran/Wide/Wide.png)

### What does it do?
Wide allows you to build your application by creating PRISM modules which participate in building the IDE. This way you can modularize the code for each part of your application.
Wide comes with 2 shells - MetroWindow and Window. The screenshot above is a MetroWindow. The one given below is a regular window.

![Screenshot](https://raw.github.com/wiki/chandramouleswaran/Wide/Wide-old.png)

Modules in this case are PRISM modules. (Note: You do NOT need to know PRISM to understand/use the framework). Check out the demo for more details.

For full details, see the [Wiki](https://github.com/TorisanKitsune/Wider/wiki).
