# TESTAR Analysis Release notes
The page covers the release notes of the new TESTAR .NET Analysis website. 

## Version 1.5.0
Version 1.5.0 includes changes in the graph element viewer. The graphics have been upgraded to adopt the style of the rest of the website. 

![Element Viewer](images/150-element-viewer.png)

Each Id in the list is now clickable and enables a full deep link in the URL. This makes it easy to share the URL with a colleague. 

![Deep linking](images/150-deep-link-elements.png)

## Version 1.4.0

### Setting screen

![Settings](images/settings-140.png)

It is now possible to specify which label to show in the graph viewer and to enable a compound layer in the viewer.

### Comparison
This version introduced an experimental feature, compare. Before it is possible to use this feature, it needs to be enabled in the settings screen. When enabled, a new option appears on the selected model view. 

![compare viewer](images/compare-option.png)

This opens a side by side graph viewer window where two graphs can be compared. More features are to come in later versions.

## Version 1.3.0
It is now possible to filter the available models in the overview. Filtering is enabled for the name, version and model identifier. 

![Search](images/search.png)

## Version 1.2.0
There is no visual upgrade in this version but a health endpoint for the docker image. To check whether the docker container is healthy, execute a GET request to `/healthz`. The result should be `Healthy`.

## Version 1.1.0
The models' overview page has got a new visual upgrade. The Logout button has been replaced with a user menu. 

During login, it is possible to access different databases. Add the name of the database in front of the username. This required to server to be set up with the multiple database feature enabled. 

![New model view](images/110-new-models-overview.png)

## Version 1.0.0
The first version of the new TESTAR .NET analysis website. It contains some early access to change strategies and the first graph viewer.

## Known issues
See all the open problems by following the below link.

[Known issues](https://github.com/TESTARtool/ChangeDetection.NET/labels/known-issue)

