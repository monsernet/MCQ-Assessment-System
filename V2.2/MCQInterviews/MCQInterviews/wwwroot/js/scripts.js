$(document).ready(function () {

    

    //Initiliazing DataTables
    $('#JobCategoriesTable').DataTable({
        paging: true,
        searching: true,
    });
    $('#jobTitlesTable').DataTable({
        paging: true,
        searching: true,
    });
    $('#jobLevelsTable').DataTable({
        paging: true,
        searching: true,
    });
    $('#mcqDiffTypesTable').DataTable({
        paging: true,
        searching: true
    });
    $('#adminDashUsersTable').DataTable({
        paging: true,
        searching: true,
        order: [[6, 'desc']]
    });
    $('#adminDashJobCategoriesTable').DataTable({
        paging: true,
        searching: true,
        order: [[3, 'desc']]
        
    }); 
    $('#adminDashJobTitlesTable').DataTable({
        paging: true,
        searching: true,
        order: [[3, 'desc']]

    }); 
    $('#adminDashJobLevelsTable').DataTable({
        paging: true,
        searching: true

    }); 
    $('#adminDashDiffAllocationsTable').DataTable({
        paging: true,
        searching: true,
        pageLength: 5
    }); 
    $('#adminDashMcqTestsTable').DataTable({
        paging: true,
        searching: true,
    });
    $('#adminDashTopeScoresTable').DataTable({
        paging: true,
        searching: true,
        buttons: [
            'copy', 
            'excel', 
            'pdf', 
            'print' 
        ]
    });
    $('#questionListTable').DataTable({
        paging: true,
        searching: true,
        pageLength: 20

    }); 
    $('#questionDiffTypesTable').DataTable({
        paging: true,
        searching: true

    }); 
    $('#McqListTable').DataTable({
        paging: true,
        searching: true,
        pageLength: 20

    });
    $('#difficultyAllocationsTable').DataTable({
        paging: true,
        searching: true,
        order: [[1, 'asc'], [2, 'asc']],

    });
    $('#archivedTestsTable').DataTable({
        paging: true,
        searching: true,
        order: [[5, 'desc']],

    });

    //**** Populate the Job Title dropdown list based on selected Theme (Category)*/
    $("#ThemesDropdown").change(function () {
        var selectedThemeId = $(this).val();

        $.ajax({
            url: '/Mcq/GetJobTitles',
            type: 'GET',
            data: { themeId: selectedThemeId },
            success: function (data) {
                var jobTitlesDropdown = $("#JobTitlesDropdown");
                jobTitlesDropdown.empty();

                $.each(data, function (index, item) {
                    jobTitlesDropdown.append($('<option>', {
                        value: item.value,
                        text: item.text
                    }));
                });
            }
        });
    });

    

    $("#editForm").submit(function (e) {
        e.preventDefault();

        var totalPercentage = 0;
        $('.form-control').each(function () {
            totalPercentage += parseInt($(this).val()) || 0;
        });

        // Validate total percentage
        if (totalPercentage !== 100) {
            alert(totalPercentage > 100 ? 'Total percentage exceeds 100%' : 'Total percentage is less than 100%');
            return;
        }


        var formData = {
            McqDifficultyTypeId: $("#editMcqDifficultyTypeId").val(),
            Allocations: []
        };

        $("#editQuestionDifficultyTypes .form-group").each(function () {
            var questionDifficultyTypeId = $(this).find("input").attr("name").match(/\d+/)[0];
            var percentage = $(this).find("input").val();

            formData.Allocations.push({
                QuestionDifficultyTypeId: parseInt(questionDifficultyTypeId),
                Percentage: parseFloat(percentage)
            });
        });

        $.ajax({
            url: '/DifficultyAllocation/UpdateAllocations',
            type: 'POST',
            contentType: 'application/json',
            data: JSON.stringify(formData),
            success: function (data) {
                console.log("Updated successfully:", data);
                $("#editModal").modal('hide');
                 location.reload(); // Reload the page to reflect changes
            },
            error: function (xhr, status, error) {
                console.error("Error updating allocations:", xhr.responseText);
                alert("Error updating allocations.");
            }
        });
    });
});

function toggleUserStatus(userId, isActive) {
    // Display a confirmation alert
    var confirmation = confirm('Are you sure you want to toggle the user status?');
    
    if (confirmation) {
        alert(userId);
        alert(isActive);
        $.ajax({
            url: '@Url.Action("ToggleUserStatus", "AdminDashboard")',
            type: 'POST',
            data: { userId: userId, isActive: isActive },
            success: function (result) {
                // Update the button text based on the new user status
                if (result.userFound == "yes") {
                    var button = $('[data-user-id="' + userId + '"]');
                    if (result.isActive) {
                        button.html('Deactivate');
                    } else {
                        button.html('Activate');
                    }
                } else {
                    alert("user not found");
                }
                
            },
            error: function (xhr, status, error) {
                // Display the error details in an alert
                var errorMessage = xhr.responseText || 'Unknown error occurred.';
                alert('Error toggling user status: ' + errorMessage);
            }
        });
    }
}

function openEditModal(mcqDifficultyTypeId) {
    $("#editMcqDifficultyTypeId").val(mcqDifficultyTypeId); // Set hidden field value

    // Fetch data for the selected MCQ difficulty type Name
    $.ajax({
        url: '/McqDifficultyType/GetMcqDifficultyTypeName', // Replace with your actual URL
        data: { mcqDifficultyTypeId: mcqDifficultyTypeId },
        success: function (data) {
            $("#editModalMcqDifficultyType").val(data);
        },
        error: function (error) {
            console.error("Error fetching MCQ Difficulty Type Name:", error);
        }
    });

    // Fetch data for the difficulty Allocations
    $.ajax({
        url: '/DifficultyAllocation/GetDifficultyAllocations', 
        data: { mcqDifficultyTypeId: mcqDifficultyTypeId },
        success: function (data) {
            var questionDifficultyTypesHtml = "<div class='row'>"; 

            data.forEach(function (allocation) {
                questionDifficultyTypesHtml += `
                    <div class="col-4 mb-2">
                        <div class="form-group">
                            <label class="form-label" for="questionDifficultyTypeId-${allocation.questionDifficultyTypeId}">${allocation.questionDifficultyTypeName}</label>
                            <div class="input-group">
                                <input type="number" class="form-control" id="questionDifficultyTypeId-${allocation.questionDifficultyTypeId}" name="Percentages[${allocation.questionDifficultyTypeId}]" value="${allocation.percentage}" min="0" max="100" />
                                <span class="input-group-text">%</span>
                            </div>
                        </div>
                    </div>`;
            });
            questionDifficultyTypesHtml += "</div>";

            $("#editQuestionDifficultyTypes").html(questionDifficultyTypesHtml);
            
            
        },
        error: function (error) {
            console.error("Error fetching difficulty allocations:", error);
        }
    });

    $("#editModal").modal("show");
}

