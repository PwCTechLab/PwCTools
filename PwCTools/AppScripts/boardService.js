sulhome.kanbanBoardApp.service('boardService', function ($http, $q, $rootScope) {
    var proxy = null;

    var getColumns = function (projectIdVal) {
        return $http.get("/api/BoardWebApi/Get", { params: { projectId: projectIdVal } })
            .then(function (response) {
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

    var getProjectUsers = function () {
        return $http.get("/api/BoardWebApi/GetProjectUsers")
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

    var editTask = function (taskIdVal, taskNameVal, taskAssigneeVal, taskDueDateVal, taskDescriptionVal) {
        return $http.post("/api/BoardWebApi/EditTask", { taskId: taskIdVal, taskName: taskNameVal, taskAssignee: taskAssigneeVal, taskDueDate: taskDueDateVal, taskDescription: taskDescriptionVal })
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
        this.proxy = $.connection.KanbanBoard;

        var Id = $("#ProjectId").val();
        $.connection.hub.qs = 'projectId=' + Id;

        // Listen to the 'BoardUpdated' event that will be pushed from SignalR server
        this.proxy.client.BoardUpdated = function (name, message) {
            $rootScope.$emit("refreshBoard");
        };

        // Connecting to SignalR server        
        return $.connection.hub.start()
        .then(function (connectionObj) {
            return connectionObj;
        }, function (error) {
            return error.message;
        });
    };

    // Call 'NotifyBoardUpdated' on SignalR server
    var sendRequest = function () {
        this.proxy.invoke('NotifyBoardUpdated', $("#ProjectId").val());
        $rootScope.$emit("refreshBoard");
    };

    var updateProject = function (oldProjectId, newProjectId) {
        return this.proxy.server.leaveGroup(oldProjectId)
        .then(function () {
            return this.server.joinGroup(newProjectId);;
        }, function (error) {
            return error.message;
        });
    };

    return {
        initialize: initialize,
        addComment: addComment,
        getProjectUsers: getProjectUsers,
        deleteTask: deleteTask,
        archiveTask: archiveTask,
        editTask: editTask,
        sendRequest: sendRequest,
        updateProject: updateProject,
        getComments: getComments,
        getColumns: getColumns,
        canMoveTask: canMoveTask,
        moveTask: moveTask
    };
});