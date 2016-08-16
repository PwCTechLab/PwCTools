sulhome.kanbanBoardApp.service('boardService', function ($http, $q, $rootScope) {
    var proxy = null;

    var getColumns = function () {
        return $http.get("/api/BoardWebApi/Get").then(function (response) {
            return response.data;
        }, function (error) {
            return $q.reject(error.data.Message);
        });
    };

    var getComments = function (taskIdVal) {
        return $http.get("/api/BoardWebApi/GetComments", { params: { taskId: taskIdVal } })
            .then(function (response) {
            return response.data;
        }, function (error) {
            return $q.reject(error.data.Message);
        });
    };

    var canMoveTask = function (sourceColIdVal, targetColIdVal) {
        return $http.get("/api/BoardWebApi/CanMove", { params: { sourceColId: sourceColIdVal, targetColId: targetColIdVal } })
            .then(function (response) {
                return response.data.canMove;
            }, function (error) {
                return $q.reject(error.data.Message);
            });
    };

    var moveTask = function (taskIdVal, targetColIdVal) {
        return $http.post("/api/BoardWebApi/MoveTask", { taskId: taskIdVal, targetColId: targetColIdVal })
            .then(function (response) {
                return response.status == 200;
            }, function (error) {
                return $q.reject(error.data.Message);
            });
    };

    var editTask = function (taskIdVal, taskNameVal, taskDescriptionVal) {
        return $http.post("/api/BoardWebApi/EditTask", { taskId: taskIdVal, taskName: taskNameVal, taskDescription: taskDescriptionVal })
            .then(function (response) {
                return response.status == 200;
            }, function (error) {
                return $q.reject(error.data.Message);
            });
    };

    var archiveTask = function (taskIdVal) {
        return $http.post("/api/BoardWebApi/ArchiveTask", { taskId: taskIdVal })
            .then(function (response) {
                return response.status == 200;
            }, function (error) {
                return $q.reject(error.data.Message);
            });
    };

    var deleteTask = function (taskIdVal) {
        return $http.post("/api/BoardWebApi/DeleteTask", { taskId: taskIdVal })
            .then(function (response) {
                return response.status == 200;
            }, function (error) {
                return $q.reject(error.data.Message);
            });
    };

    var addComment = function (taskIdVal, commentVal, commentIdVal) {
        return $http.post("/api/BoardWebApi/AddComment", { taskId: taskIdVal, comment: commentVal, commentId: commentIdVal })
            .then(function (response) {
                return response.status == 200;
            }, function (error) {
                return $q.reject(error.data.Message);
            });
    };

    var initialize = function () {

        connection = jQuery.hubConnection();
        this.proxy = connection.createHubProxy('KanbanBoard');

        // Listen to the 'BoardUpdated' event that will be pushed from SignalR server
        this.proxy.on('BoardUpdated', function () {
            $rootScope.$emit("refreshBoard");
        });

        // Connecting to SignalR server        
        return connection.start()
        .then(function (connectionObj) {
            return connectionObj;
        }, function (error) {
            return error.message;
        });
    };

    // Call 'NotifyBoardUpdated' on SignalR server
    var sendRequest = function () {
        this.proxy.invoke('NotifyBoardUpdated');
    };

    return {
        initialize: initialize,
        addComment: addComment,
        deleteTask: deleteTask,
        archiveTask: archiveTask,
        editTask: editTask,
        sendRequest: sendRequest,
        getComments: getComments,
        getColumns: getColumns,
        canMoveTask: canMoveTask,
        moveTask: moveTask
    };
});