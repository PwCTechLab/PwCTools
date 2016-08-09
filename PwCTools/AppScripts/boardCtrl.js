sulhome.kanbanBoardApp.controller('boardCtrl', function ($scope, boardService) {
    // Model
    $scope.columns = [];
    $scope.isLoading = false;

    function init() {
        $scope.isLoading = true;
        boardService.initialize().then(function (data) {
            $scope.isLoading = false;
            $scope.refreshBoard();
        }, onError);
    };

    $scope.refreshBoard = function refreshBoard() {
        $scope.isLoading = true;
        boardService.getColumns()
           .then(function (data) {
               $scope.isLoading = false;
               $scope.columns = data;
           }, onError);
    };

    $scope.loadTaskDetails = function loadTaskDetails(id) {
        //$scope.isLoading = true;
        boardService.getComments(id)
           .then(function (data) {
               //$scope.isLoading = false;
               $scope.comments = data;
           }, onError);
    };

    $scope.onDrop = function (data, targetColId) {
        boardService.canMoveTask(data.ColumnId, targetColId)
            .then(function (canMove) {
                if (canMove) {
                    boardService.moveTask(data.Id, targetColId).then(function (taskMoved) {
                        $scope.isLoading = false;
                        boardService.sendRequest();
                    }, onError);
                    $scope.isLoading = true;
                }

            }, onError);
    };

    //Edit Task
    $scope.save = function () {
        boardService.editTask($('#task-id').val(), $('#task-name').val(), $('#task-description').val()).then(function (taskEdited) {
            $scope.isLoading = false;
            boardService.sendRequest();
        }, onError);
        $scope.isLoading = true;
        $('#updateTaskModal').modal('hide');
    };

    //Archive Task
    $scope.archive = function (id) {
            bootbox.confirm("Are you sure want to archive this task?", function (result) {
                if (result) {
                    boardService.archiveTask(id).then(function (taskArchived) {
                        $scope.isLoading = false;
                        boardService.sendRequest();
                    }, onError);
                    $scope.isLoading = true;
                }
            });
    };

    //Delete Task
    $scope.delete = function () {
        bootbox.confirm("Are you sure want to delete this task?", function (result) {
            if (result) {
                boardService.deleteTask($('#task-id').val()).then(function (taskDeleted) {
                    $scope.isLoading = false;
                    boardService.sendRequest();
                }, onError);
                $scope.isLoading = true;
                $('#updateTaskModal').modal('hide');
            }
        });
    };

    //Add Comment
    $scope.addComment = function () {
        boardService.addComment($('#task-comment-id').val(), $('#task-comment').val()).then(function (addComment) {
            $scope.isLoading = false;
            $('#comment-input-container').toggle();
            $('#task-comment').val('');
            boardService.getComments($('#task-comment-id').val())
           .then(function (data) {
               $scope.comments = data;
               boardService.sendRequest();
           }, onError);
        }, onError);
        $scope.isLoading = true;
    };

    // Listen to the 'refreshBoard' event and refresh the board as a result
    $scope.$parent.$on("refreshBoard", function (e) {
        $scope.refreshBoard();
        toastr.success("Board updated successfully", "Success");
    });

    var onError = function (errorMessage) {
        $scope.isLoading = false;
        toastr.error(errorMessage, "Error");
    };

    init();
});

$(document).ready(function () {

    $('#updateTaskModal').on('show.bs.modal', function (event) {
        var button = $(event.relatedTarget) // Button that triggered the modal
        var id = button.data('custom-id') // Extract info from data-* attributes
        var name = button.data('name')
        var description = button.data('description')
        
        var modal = $(this)
        modal.find('#task-id').val(id)
        modal.find('#task-name').val(name)
        modal.find('#task-description').val(description)
    })

    $('#taskDetailsModal').on('show.bs.modal', function (event) {
        //Set Scrolling
        var width = $('.modal-dialog').width() - 22;
        $(this).find("#comment-container").css("max-width", width);
        
        var button = $(event.relatedTarget) // Button that triggered the modal
        var id = button.data('custom-id') // Extract info from data-* attributes
        var name = button.data('name')
        var description = button.data('description')

        var modal = $(this)
        modal.find('#task-comment-id').val(id)
        modal.find('#task-name').text(name + ' Details')
        modal.find('#task-description').text(description)
    })

    $(document).on("click", "#btnEditTask", function (event) {
        $('#editTaskModalTitle').text('Edit Task');
    })

    $("#btnAddTask").on("click", function () {
        $('#editTaskModalTitle').text('Create Task');
    })

});
