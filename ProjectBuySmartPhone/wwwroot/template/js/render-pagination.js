function renderPagination(response, id, callbackFunc) {
    const {totalRecords, pageIndex, pageSize} = response;
    console.log("starting pagination", totalRecords, pageIndex, pageSize, id);
    const totalPages = Math.ceil(totalRecords / pageSize);
    console.log('total pages: ', totalPages);
    const $pagination = $(id);
    $pagination.empty();

    if (totalPages <= 1) return;

    $pagination.append(`
        <li class="page-item first ${pageIndex === 1 ? 'disabled' : ''}">
            <a class="page-link" href="javascript:void(0);" data-page="1">
                <i class="tf-icon ri-skip-back-mini-line ri-22px"></i>
            </a>
        </li>
        <li class="page-item prev ${pageIndex === 1 ? 'disabled' : ''}">
            <a class="page-link" href="javascript:void(0);" data-page="${pageIndex - 1}">
                <i class="tf-icon ri-arrow-left-s-line ri-22px"></i>
            </a>
        </li>
    `);

    let startPage = Math.max(1, pageIndex - 2);
    let endPage = Math.min(totalPages, pageIndex + 2);

    if (startPage > 1) {
        $pagination.append(`<li class="page-item"><a class="page-link" href="javascript:void(0);" data-page="1">1</a></li>`);
        if (startPage > 2) {
            $pagination.append(`<li class="page-item disabled"><span class="page-link">...</span></li>`);
        }
    }

    for (let i = startPage; i <= endPage; i++) {
        $pagination.append(`
            <li class="page-item ${i === pageIndex ? 'active' : ''}">
                <a class="page-link" href="javascript:void(0);" data-page="${i}">${i}</a>
            </li>
        `);
    }

    if (endPage < totalPages) {
        if (endPage < totalPages - 1) {
            $pagination.append(`<li class="page-item disabled"><span class="page-link">...</span></li>`);
        }
        $pagination.append(`<li class="page-item"><a class="page-link" href="javascript:void(0);" data-page="${totalPages}">${totalPages}</a></li>`);
    }

    // Tạo nút "Next" và "Last"
    $pagination.append(`
        <li class="page-item next ${pageIndex === totalPages ? 'disabled' : ''}">
            <a class="page-link" href="javascript:void(0);" data-page="${pageIndex + 1}">
                <i class="tf-icon ri-arrow-right-s-line ri-22px"></i>
            </a>
        </li>
        <li class="page-item last ${pageIndex === totalPages ? 'disabled' : ''}">
            <a class="page-link" href="javascript:void(0);" data-page="${totalPages}">
                <i class="tf-icon ri-skip-forward-mini-line ri-22px"></i>
            </a>
        </li>
    `);

    // Sự kiện click thay đổi trang
    $('.pagination .page-link').on('click', function () {
        const newPage = parseInt($(this).data('page'));
        if (!isNaN(newPage) && newPage !== pageIndex) {
            callbackFunc(totalRecords, newPage, pageSize);
        }
    });
    console.log("end pagination");
}

// response = { totalRecords, pageIndex, pageSize }
// ulSelector = '#myPagination' (UL cần render, nhớ có class "pagination")
// onPageChange = function(totalRecords, newPage, pageSize) { ... }
function renderPaginationBSB(response, ulSelector, onPageChange) {
    const { totalRecords, pageIndex, pageSize } = response;
    const totalPages = Math.ceil(totalRecords / pageSize);
    const $pagination = $(ulSelector);

    $pagination.empty();                          // xoá cũ
    if (totalPages <= 1) return;

    // helper tạo li
    const li = (cls, page, content, disabled = false, active = false) => {
        const dis = disabled ? 'disabled' : '';
        const act = active ? 'active' : '';
        const aria = disabled ? 'aria-disabled="true" tabindex="-1"' : '';
        return `
      <li class="${cls} ${dis} ${act}">
        <a class="page-link waves-effect" href="javascript:void(0);" data-page="${page}" ${aria}>
          ${content}
        </a>
      </li>`;
    };

    // First / Prev
    $pagination.append(li('page-item first', 1,
        `<i class="material-icons">first_page</i>`,
        pageIndex === 1));
    $pagination.append(li('page-item prev', pageIndex - 1,
        `<i class="material-icons">chevron_left</i>`,
        pageIndex === 1));

    // range
    let startPage = Math.max(1, pageIndex - 2);
    let endPage = Math.min(totalPages, pageIndex + 2);

    if (startPage > 1) {
        $pagination.append(li('page-item', 1, '1'));
        if (startPage > 2) {
            $pagination.append(`<li class="page-item disabled">
        <span class="page-link page-ellipsis">…</span>
      </li>`);
        }
    }

    for (let i = startPage; i <= endPage; i++) {
        $pagination.append(li('page-item', i, String(i), false, i === pageIndex));
    }

    if (endPage < totalPages) {
        if (endPage < totalPages - 1) {
            $pagination.append(`<li class="page-item disabled">
        <span class="page-link page-ellipsis">…</span>
      </li>`);
        }
        $pagination.append(li('page-item', totalPages, String(totalPages)));
    }

    // Next / Last
    $pagination.append(li('page-item next', pageIndex + 1,
        `<i class="material-icons">chevron_right</i>`,
        pageIndex === totalPages));
    $pagination.append(li('page-item last', totalPages,
        `<i class="material-icons">last_page</i>`,
        pageIndex === totalPages));

    // Bind click trong phạm vi UL (tránh trùng)
    $pagination.off('click', 'a.page-link').on('click', 'a.page-link', function () {
        const newPage = parseInt($(this).data('page'), 10);
        const isDisabled = $(this).closest('li').hasClass('disabled');
        const isActive = $(this).closest('li').hasClass('active');
        if (!isNaN(newPage) && !isDisabled && !isActive && typeof onPageChange === 'function') {
            onPageChange(totalRecords, newPage, pageSize);
        }
    });
}
