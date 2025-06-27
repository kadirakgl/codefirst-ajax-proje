document.addEventListener('DOMContentLoaded', function () {
    // Yorum ekleme
    const addCommentForm = document.getElementById('addCommentForm');
    if (addCommentForm) {
        addCommentForm.addEventListener('submit', function (e) {
            e.preventDefault();
            const postId = addCommentForm.getAttribute('data-post-id');
            const formData = new FormData(addCommentForm);
            fetch('/Comment/Add', {
                method: 'POST',
                body: new URLSearchParams({
                    postId: postId,
                    author: formData.get('author'),
                    text: formData.get('text')
                })
            })
            .then(response => response.text())
            .then(html => {
                document.getElementById('commentList').innerHTML = html;
                addCommentForm.reset();
            });
        });
    }

    // Yorum silme (delegasyon ile)
    document.getElementById('commentList').addEventListener('click', function (e) {
        if (e.target.classList.contains('delete-comment')) {
            const commentId = e.target.getAttribute('data-id');
            fetch('/Comment/Delete/' + commentId, {
                method: 'POST'
            })
            .then(response => response.text())
            .then(html => {
                document.getElementById('commentList').innerHTML = html;
            });
        }
    });
}); 