/*=============================================================================
File View Implementation
==============================================================================*/
var fileViewImpl = function (baseUrl, file, oid) {
    //-------------------------------------------------------------------------
    // Render Html
    //-------------------------------------------------------------------------
    var renderHtml = function () {
        var strFileView = '<form class="viewFilesContainer" action="' + baseUrl + '/ApproveTask/ApproveTask/Document" method="post">' +
'           <input type="submit" name="viewDocument" value="'+ file +'" class="linkBoxColor">' +
'           <input length="36" name="DocumentOid" type="hidden" value="'+ oid +'">' +
'       </form>';

        return strFileView;
    };

    return {
        //---------------------------------------------------------------------
        // Init
        //---------------------------------------------------------------------
        init: function () {

        },
        //-------------------------------------------------------------------------
        // Render
        //-------------------------------------------------------------------------
        render: function () {
            return renderHtml();
        }
    }
}