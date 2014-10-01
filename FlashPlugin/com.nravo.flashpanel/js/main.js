/*jslint vars: true, plusplus: true, devel: true, nomen: true, regexp: true, indent: 4, maxerr: 50 */
/*global $, window, location, CSInterface, SystemPath, themeManager*/

(function () {
    'use strict';

    var csInterface = new CSInterface();
        
    // Reloads extension panel
    function reloadPanel() {
        location.reload();
    }
    
    function init() {
                
        themeManager.init();
        
        $("#btn_reload").click(reloadPanel);
        
        $("#btn_export").click(function () {
            csInterface.evalScript('linkageSet()');
        });
        
        $("#btn_clear").click(function () {
            csInterface.evalScript('linkageClear()');
        });
    }
        
    init();

}());
    
