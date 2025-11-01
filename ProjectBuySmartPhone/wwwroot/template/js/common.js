function showLoading() {
    const loader = document.querySelector('.page-loader-wrapper');
    if (loader) {
        loader.style.display = 'block';
    }
}

function hideLoading() {
    const loader = document.querySelector('.page-loader-wrapper');
    if (loader) {
        loader.style.display = 'none';
    }
}