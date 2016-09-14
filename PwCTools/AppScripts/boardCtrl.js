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

    $scope.fillProjectUsers = function fillProjectUsers(assignedToID) {
        $scope.AssignedToID = assignedToID;
        boardService.getProjectUsers()
           .then(function (data) {
               $scope.users = data;

               if ($scope.AssignedToID != null)
               {
                   //Set the selected assignee
                   $.each(data, function (index, obj) {
                       if (obj.Id == $scope.AssignedToID) {
                           $("#task-assignee option:selected").removeAttr("selected");
                           $scope.user = data[index];
                           return false;
                       }
                   });

               }
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
        boardService.editTask($('#task-id').val(), $('#task-name').val(), $('#task-assignee').val(), $('#task-duedate').val(), $('#task-description').val()).then(function (taskEdited) {
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
        boardService.addComment($('#detailDlg-task-id').val(), $('#detailDlg-new-comment').val(), $('#detailDlg-comment-id').val()).then(function (addComment) {
            $scope.isLoading = false;
            $('#comment-input-container').toggle();
            $('#detailDlg-new-comment').val('');
            boardService.getComments($('#detailDlg-task-id').val())
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
        var button = $(event.relatedTarget); // Button that triggered the modal
        var id = button.data('custom-id'); // Extract info from data-* attributes
        var name = button.data('name');
        var description = button.data('description');
        var duedate = button.data('duedate');
        
        var modal = $(this);
        modal.find('#task-id').val(id);
        modal.find('#task-name').val(name);
        if (duedate != "" && duedate != null)
            modal.find('#task-duedate').val($.datepicker.formatDate('mm/dd/yy', new Date(duedate)));
        modal.find('#task-description').val(description);
    })

    $('#taskDetailsModal').on('show.bs.modal', function (event) {
        //Set Scrolling
        var width = $('.modal-dialog').width() - 22;
        $(this).find("#comment-container").css("max-width", width);
        
        var button = $(event.relatedTarget); // Button that triggered the modal
        var id = button.data('custom-id'); // Extract info from data-* attributes
        var name = button.data('name');
        var description = button.data('description');

        var modal = $(this);
        modal.find('#detailDlg-task-id').val(id);
        modal.find('#task-name').text(name + ' Details');
        modal.find('#task-description').text(description);
    })

    $(document).on("click", "#btnEditTask", function (event) {
        $('#editTaskModalTitle').text('Edit Task');
    })

    $("#btnAddTask").on("click", function () {
        $('#editTaskModalTitle').text('Create Task');
    })

    $(function () {
        $("#task-duedate").datepicker();
    });

    //File Upload Script
    $('#btnFileUpload').fileupload({
        url: '../Handlers/FileUploadHandler.ashx?upload=start',
        dataType: 'json',
        add: function (e, data) {
            console.log('add', data);
            $('#progressbar div').css('width', '0%');
            $('#progress').show();
            $('#comment-container').animate({ scrollTop: $('#modal-footer').offset().top }, 'slow'); //scroll to bottom
            data.submit();
    },
    progress: function(e, data) {
        var progress = parseInt(data.loaded / data.total * 100, 10);
        $('.progress-bar').css('width', progress + '%').attr('aria-valuenow', progress);
    },
    success: function(response, status) {
        //$('#progressbar').hide();
        //$('#progressbar div').css('width', '0%');
        $('#detailDlg-comment-id').val(response.CommentId);
        console.log('success', response);
    },
    error: function(error) {
        $('#progress').hide();
        $('#progressbar div').css('width', '0%');
        console.log('error', error);
    }
    });

    $('#addComment').bind('fileuploadsubmit', function (e, data) {
        // The example input, doesn't have to be part of the upload form:
        var input = $('#detailDlg-task-id');
        data.formData = { taskId: input.val() };
    });

});
