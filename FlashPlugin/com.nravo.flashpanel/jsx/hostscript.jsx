/*jslint vars: true, plusplus: true, devel: true, nomen: true, regexp: true, indent: 4, maxerr: 50 */
/*global $, Folder*/

function removeSpaces(str)
{
    return str.replace(/\s+/g, '');
}

function getDocument()
{
    return fl.getDocumentDOM();
}

function getLibrary()
{ 
    return getDocument().library;
}

function sayHello(){
    alert("hello from ExendScript");
}

function linkageSet()
{
    fl.trace("> Set Linkages");
    
    var items = getLibrary().getSelectedItems();
    
    var getClassName = function(path)
    {
        var parts = path.split("/");
        var packages = [];
        for (var i = 0; i < parts.length; i++)
        {
            var part = removeSpaces(parts[i]);
            var prefix = part.substring(0, 1);
            if (prefix != "-")
            packages.push(part);
        }
        return packages.join(".");
    }
    
    for each (var item in items)
    {
        if (item.itemType == "folder")
            continue;
            
        var className = getClassName(item.name);
    
        var baseClass = '';
        if (item.itemType != 'movie clip'
            && item.itemType != 'button'
            && item.itemType != 'bitmap'
            && item.itemType != 'font'
            && item.itemType != 'sound'
            && item.itemType != 'component'
        )
        {
            continue;
        }
        
        if (item.linkageImportForRS == true)
        {
            item.linkageImportForRS = false;
        }
        item.linkageExportForAS = true;
        item.linkageExportForRS = false;
        item.linkageExportInFirstFrame = true;
        item.linkageClassName = className;
        
        fl.trace(className);
    }
    
    //libRefreshSelection();
}

function linkageClear()
{
    fl.trace("> Clear Linkage");
    
    var items = getLibrary().getSelectedItems();
    for each (var item in items)
    {
        if (item.itemType == "folder")
            continue;
            
        if (item.linkageExportForAS == true)
        {
            item.linkageClassName = "";
            item.linkageExportForAS = false;
        }
    }
}