function AddClickEvents() {
    //remove the event first
    $('#addCategoryItem').unbind('click');
    $("#addCategoryItem").click(function () {
        $.ajax({
            url: this.href,
            cache: false,
            success: function (html) {
                $("#div_Categories").append(html);
                AddClickEvents();
            },
            error: function (html) {
                alert(html);
            }
        });
        return false;
    });

    //remove the event first
    $('.addSubcategoryItem').unbind('click');
    $(".addSubcategoryItem").click(handleNewItemClick);

    //remove the event first
    $('.addProductItem').unbind('click');
    $(".addProductItem").click(handleNewItemClick);

    function handleNewItemClick() {
        
            var parent = this.parentElement;
            var formData = {
                id: parent.id
            };
            $.ajax({
                type: "POST",
                url: this.href,
                data: JSON.stringify(formData),
                contentType: "application/json; charset=utf-8",
                success: function (data) {
                    $(parent).append(data);
                    AddClickEvents();
                },
                error: function (data) {
                    alert(data);
                }
            });
            return false;
    }
}