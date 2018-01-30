var HandleIO = {
    WindowAlert : function (message) {
        window.alert(Pointer_stringify(message));
    },
    SyncFiles : function () {
        FS.syncfs(false, function (err) {
            if (err) console.log(err);
        });
    }
};

mergeInto(LibraryManager.library, HandleIO);
