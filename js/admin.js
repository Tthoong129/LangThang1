document.addEventListener('DOMContentLoaded', () => {
    initAdmin();
});

let currentRejectId = null;
let currentRejectType = null; // 'place' or 'edit'

let growthChartInstance = null;
let donutChartInstance = null;
let currentTimeFilter = '30d';

// Mock Top Places Data for Analytics
const mockTopPlacesData = [
    { id: 1, rank: 1, name: 'VinWonders & Cáp treo Hòn Thơm', province: 'Kiên Giang', rating: 4.9, reviews: 1420, favs: 2150, image: 'https://images.unsplash.com/photo-1540555700478-4be289fbecef?w=400' },
    { id: 2, rank: 2, name: 'Sun World Bà Nà Hills & Cầu Vàng', province: 'Đà Nẵng', rating: 4.8, reviews: 1180, favs: 1890, image: 'https://images.unsplash.com/photo-1559592413-7cec4d0cae2b?w=400' },
    { id: 3, rank: 3, name: 'Cà phê Giảng (Cà phê trứng Hà Nội)', province: 'Hà Nội', rating: 4.8, reviews: 960, favs: 1420, image: 'https://images.unsplash.com/photo-1501339847302-ac426a4a7cbb?w=400' },
    { id: 4, rank: 4, name: 'Bánh Mì Huỳnh Hoa Sài Gòn', province: 'TP. Hồ Chí Minh', rating: 4.7, reviews: 890, favs: 1250, image: 'https://images.unsplash.com/photo-1509722747041-616f39b57569?w=400' }
];

// Fallback Mock Datasets for Action Queue & Subtabs
let fallbackPendingPlaces = [
    {
        id: 101,
        name: 'Cà phê Giảng (Cà phê trứng Hà Nội)',
        categoryName: 'Quán Cà phê & Trà',
        proposer: { fullName: 'Trần Văn Hùng' },
        createdAt: new Date(Date.now() - 3600000 * 5).toISOString(),
        address: '39 Nguyễn Hữu Huân, Hoàn Kiếm, Hà Nội'
    },
    {
        id: 102,
        name: 'Bánh Mì Huỳnh Hoa Sài Gòn',
        categoryName: 'Nhà hàng & Quán ăn',
        proposer: { fullName: 'Nguyễn Thị Mai' },
        createdAt: new Date(Date.now() - 3600000 * 18).toISOString(),
        address: '26 Lê Thị Riêng, P. Bến Thành, Quận 1, TP.HCM'
    },
    {
        id: 103,
        name: 'Hồ Tuyền Lâm & Rừng Thông Đà Lạt',
        categoryName: 'Danh lam thắng cảnh',
        proposer: { fullName: 'Lê Minh Tuấn' },
        createdAt: new Date(Date.now() - 3600000 * 36).toISOString(),
        address: 'Phường 4, TP. Đà Lạt, Lâm Đồng'
    }
];

let fallbackPendingEdits = [
    {
        id: 201,
        placeName: 'Phở Thìn Lò Đúc',
        proposerName: 'Hoàng Anh',
        submittedAt: new Date(Date.now() - 3600000 * 8).toISOString(),
        proposedData: { openingHours: '06:00 - 21:30', minPrice: 70000, maxPrice: 120000 }
    },
    {
        id: 202,
        placeName: 'Bà Nà Hills Đà Nẵng',
        proposerName: 'Phạm Duy',
        submittedAt: new Date(Date.now() - 3600000 * 24).toISOString(),
        proposedData: { phone: '1900 1888', website: 'https://banahills.sunworld.vn' }
    }
];

let fallbackReports = [
    {
        id: 301,
        targetTitle: 'Đánh giá tại Quán Ăn Ngon',
        reporterName: 'Đặng Thu Thảo',
        submittedAt: new Date(Date.now() - 3600000 * 12).toISOString(),
        reasonContent: 'Ngôn từ xúc phạm, nội dung không liên quan đến trải nghiệm ẩm thực thực tế.'
    },
    {
        id: 302,
        targetTitle: 'Hình ảnh tại Bãi Sao Phú Quốc',
        reporterName: 'Vũ Quốc Bảo',
        submittedAt: new Date(Date.now() - 3600000 * 28).toISOString(),
        reasonContent: 'Hình ảnh quảng cáo spam sản phẩm thương mại khác.'
    }
];

let fallbackAppeals = [
    {
        id: 401,
        targetType: 'Địa điểm',
        targetId: 12,
        userName: 'Chủ quán Cà phê Trứng',
        submittedAt: new Date(Date.now() - 3600000 * 15).toISOString(),
        reason: 'Yêu cầu mở lại địa điểm đã bị báo cáo nhầm địa chỉ kinh doanh.'
    }
];

let fallbackAuditLogs = [
    { userName: 'Admin Nguyễn A', action: 'Đã phê duyệt địa điểm "Cà phê Giảng"', targetType: 'Place', targetId: '101', description: 'Kiểm duyệt hợp lệ đầy đủ hình ảnh & thông tin', createdAt: new Date(Date.now() - 60000 * 5).toISOString() },
    { userName: 'Admin Trần B', action: 'Đã phân công danh mục "Quán Cà Phê" cho Admin C', targetType: 'Category', targetId: '4', description: 'Phân quyền quản lý danh mục ẩm thực', createdAt: new Date(Date.now() - 60000 * 18).toISOString() },
    { userName: 'Hệ thống', action: 'Có 1 địa điểm mới được người dùng đề xuất', targetType: 'Place', targetId: '103', description: 'Chờ ban quản trị kiểm tra', createdAt: new Date(Date.now() - 60000 * 32).toISOString() },
    { userName: 'User Nguyễn Văn A', action: 'Đã đăng ký tài khoản thành viên mới', targetType: 'User', targetId: '12580', description: 'Xác thực qua email thành công', createdAt: new Date(Date.now() - 60000 * 45).toISOString() }
];

// --------------------------------------------------------------------------
// 1. INITIALIZATION & AUTH
// --------------------------------------------------------------------------
async function initAdmin() {
    let user = typeof api !== 'undefined' ? api.getCurrentUser() : null;
    if (!user || !user.role || !user.role.includes('admin')) {
        user = {
            id: 1,
            fullName: 'Admin Hệ Thống',
            email: 'admin@travelreview.vn',
            role: 'system_admin'
        };
    }

    const name = user.fullName || 'Admin Hệ Thống';
    const initials = name.split(' ').map(n => n[0]).slice(-2).join('').toUpperCase() || 'AD';
    
    const initialsEl = document.getElementById('admin-initials');
    const nameEl = document.getElementById('admin-name');
    const roleEl = document.getElementById('admin-role');

    if (initialsEl) initialsEl.textContent = initials;
    if (nameEl) nameEl.textContent = name;
    if (roleEl) roleEl.textContent = user.role === 'system_admin' ? 'Admin Hệ Thống (System Admin)' : 'Admin Danh Mục (Category Admin)';

    if (user.role !== 'system_admin') {
        const auditMenu = document.getElementById('menu-auditlogs');
        if (auditMenu && auditMenu.parentElement) {
            auditMenu.parentElement.style.display = 'none';
        }
    }

    switchTab('dashboard');
}

// --------------------------------------------------------------------------
// 2. TAB SWITCHING
// --------------------------------------------------------------------------
function switchTab(tabId, event) {
    if (event) event.preventDefault();
    
    document.querySelectorAll('.admin-menu a').forEach(a => a.classList.remove('active'));
    if (event && event.currentTarget) {
        event.currentTarget.classList.add('active');
    } else {
        const link = document.querySelector(`.admin-menu a[onclick*="switchTab('${tabId}'"]`);
        if (link) link.classList.add('active');
    }

    const dashboardView = document.getElementById('dashboard-view');
    const tableView = document.getElementById('table-view');
    const timeFilters = document.getElementById('dashboard-time-filters');

    if (tabId === 'dashboard') {
        document.getElementById('page-title').textContent = 'Xin chào, Admin 👋';
        document.getElementById('page-sub').textContent = 'Tổng quan sức khỏe và hoạt động hệ thống Lang Thang';
        if (dashboardView) dashboardView.style.display = 'block';
        if (tableView) tableView.style.display = 'none';
        if (timeFilters) timeFilters.style.display = 'flex';
        loadDashboard();
    } else {
        if (dashboardView) dashboardView.style.display = 'none';
        if (tableView) tableView.style.display = 'block';
        if (timeFilters) timeFilters.style.display = 'none';
        
        if (tabId === 'places') {
            document.getElementById('page-title').textContent = 'Quản lý địa điểm chờ duyệt';
            document.getElementById('page-sub').textContent = 'Kiểm tra thông tin, hình ảnh và phê duyệt địa điểm mới';
            loadPendingPlaces();
        } else if (tabId === 'edits') {
            document.getElementById('page-title').textContent = 'Đề xuất chỉnh sửa thông tin';
            document.getElementById('page-sub').textContent = 'Các cập nhật thông tin địa điểm từ cộng đồng du khách';
            loadPendingEdits();
        } else if (tabId === 'reports') {
            document.getElementById('page-title').textContent = 'Báo cáo vi phạm từ người dùng';
            document.getElementById('page-sub').textContent = 'Xử lý phản ánh về nội dung xúc phạm, spam hoặc thông tin sai';
            loadReports();
        } else if (tabId === 'appeals') {
            document.getElementById('page-title').textContent = 'Quản lý khiếu nại & Hỗ trợ';
            document.getElementById('page-sub').textContent = 'Xem xét giải trình và mở lại bài viết/địa điểm';
            loadAppeals();
        } else if (tabId === 'auditlogs') {
            document.getElementById('page-title').textContent = 'Nhật ký hoạt động hệ thống (Audit Logs)';
            document.getElementById('page-sub').textContent = 'Truy vết toàn bộ thao tác của Quản trị viên và Người dùng';
            loadAuditLogs();
        }
    }
}

// --------------------------------------------------------------------------
// 3. DASHBOARD ANALYTICS & CHARTS
// --------------------------------------------------------------------------
async function loadDashboard() {
    // Update KPI numbers
    const kpiUsers = document.getElementById('kpi-users');
    const kpiPlaces = document.getElementById('kpi-places');
    const kpiReviews = document.getElementById('kpi-reviews');
    const kpiReports = document.getElementById('kpi-reports');

    if (kpiUsers) kpiUsers.textContent = '12,580';
    if (kpiPlaces) kpiPlaces.textContent = '3,246';
    if (kpiReviews) kpiReviews.textContent = '8,921';
    if (kpiReports) kpiReports.textContent = '18';

    // Update Action Queue badge numbers
    const qPlaces = document.getElementById('q-places');
    const qReports = document.getElementById('q-reports');
    const qEdits = document.getElementById('q-edits');
    const qAppeals = document.getElementById('q-appeals');

    if (qPlaces) qPlaces.textContent = '24';
    if (qReports) qReports.textContent = '18';
    if (qEdits) qEdits.textContent = '12';
    if (qAppeals) qAppeals.textContent = '6';

    // Initialize Charts & Sections
    initGrowthChart();
    initCategoryDonut();
    renderTopPlaces('rating');
    renderRecentActivityStream();
}

function setTimeFilter(range, btn) {
    currentTimeFilter = range;
    document.querySelectorAll('#dashboard-time-filters .time-pill-btn').forEach(b => b.classList.remove('active'));
    if (btn) btn.classList.add('active');

    // Dynamically adjust growth chart data based on time range
    if (growthChartInstance) {
        if (range === 'today') {
            growthChartInstance.data.labels = ['00h', '04h', '08h', '12h', '16h', '20h', '23h'];
            growthChartInstance.data.datasets[0].data = [12, 5, 28, 64, 82, 53, 21];
            growthChartInstance.data.datasets[1].data = [2, 0, 4, 11, 14, 8, 3];
            growthChartInstance.data.datasets[2].data = [18, 9, 45, 98, 120, 85, 34];
        } else if (range === '7d') {
            growthChartInstance.data.labels = ['T2', 'T3', 'T4', 'T5', 'T6', 'T7', 'CN'];
            growthChartInstance.data.datasets[0].data = [180, 220, 240, 290, 340, 520, 480];
            growthChartInstance.data.datasets[1].data = [15, 22, 19, 31, 28, 45, 40];
            growthChartInstance.data.datasets[2].data = [260, 310, 350, 420, 490, 710, 680];
        } else if (range === '30d') {
            growthChartInstance.data.labels = ['Tuần 1', 'Tuần 2', 'Tuần 3', 'Tuần 4'];
            growthChartInstance.data.datasets[0].data = [1200, 1450, 1680, 1920];
            growthChartInstance.data.datasets[1].data = [140, 180, 210, 260];
            growthChartInstance.data.datasets[2].data = [1800, 2100, 2450, 2890];
        } else if (range === '6m') {
            growthChartInstance.data.labels = ['T3', 'T4', 'T5', 'T6', 'T7', 'T8'];
            growthChartInstance.data.datasets[0].data = [4200, 5600, 7100, 8900, 10800, 12580];
            growthChartInstance.data.datasets[1].data = [800, 1200, 1650, 2100, 2700, 3246];
            growthChartInstance.data.datasets[2].data = [3100, 4300, 5500, 6700, 7900, 8921];
        } else if (range === '1y') {
            growthChartInstance.data.labels = ['Q1', 'Q2', 'Q3', 'Q4'];
            growthChartInstance.data.datasets[0].data = [3200, 6500, 9800, 12580];
            growthChartInstance.data.datasets[1].data = [750, 1500, 2400, 3246];
            growthChartInstance.data.datasets[2].data = [2100, 4500, 6900, 8921];
        }
        growthChartInstance.update();
    }

    if (typeof UI !== 'undefined') {
        UI.showToast(`Đã lọc dữ liệu theo: ${btn ? btn.textContent : range}`, 'info');
    }
}

function initGrowthChart() {
    const canvas = document.getElementById('growthChartCanvas');
    if (!canvas || typeof Chart === 'undefined') return;

    if (growthChartInstance) {
        growthChartInstance.destroy();
    }

    const ctx = canvas.getContext('2d');

    growthChartInstance = new Chart(ctx, {
        type: 'line',
        data: {
            labels: ['Tuần 1', 'Tuần 2', 'Tuần 3', 'Tuần 4'],
            datasets: [
                {
                    label: 'Người dùng mới',
                    data: [1200, 1450, 1680, 1920],
                    borderColor: '#d9532f',
                    backgroundColor: 'rgba(217, 83, 47, 0.08)',
                    borderWidth: 2.5,
                    fill: true,
                    tension: 0.35,
                    pointRadius: 4,
                    pointHoverRadius: 6
                },
                {
                    label: 'Địa điểm mới',
                    data: [140, 180, 210, 260],
                    borderColor: '#0284c7',
                    backgroundColor: 'rgba(2, 132, 199, 0.06)',
                    borderWidth: 2.5,
                    fill: true,
                    tension: 0.35,
                    pointRadius: 4,
                    pointHoverRadius: 6
                },
                {
                    label: 'Đánh giá mới',
                    data: [1800, 2100, 2450, 2890],
                    borderColor: '#10b981',
                    backgroundColor: 'rgba(16, 185, 129, 0.06)',
                    borderWidth: 2.5,
                    fill: true,
                    tension: 0.35,
                    pointRadius: 4,
                    pointHoverRadius: 6
                }
            ]
        },
        options: {
            responsive: true,
            maintainAspectRatio: false,
            plugins: {
                legend: {
                    display: true,
                    position: 'bottom',
                    labels: {
                        boxWidth: 12,
                        usePointStyle: true,
                        font: { family: 'Plus Jakarta Sans', size: 12, weight: '600' }
                    }
                },
                tooltip: {
                    backgroundColor: '#1e293b',
                    titleFont: { family: 'Plus Jakarta Sans', weight: 'bold' },
                    bodyFont: { family: 'Plus Jakarta Sans' },
                    padding: 10,
                    cornerRadius: 8
                }
            },
            scales: {
                x: {
                    grid: { display: false }
                },
                y: {
                    grid: { color: '#f1f5f9' },
                    ticks: {
                        callback: function(val) {
                            return val >= 1000 ? (val / 1000) + 'k' : val;
                        }
                    }
                }
            }
        }
    });
}

function toggleChartDataset(target) {
    if (!growthChartInstance) return;

    document.querySelectorAll('.filter-tab-btn').forEach(b => b.classList.remove('active'));
    
    if (target === 'all') {
        document.getElementById('btn-chart-all')?.classList.add('active');
        growthChartInstance.data.datasets.forEach(ds => ds.hidden = false);
    } else {
        if (target === 0) document.getElementById('btn-chart-users')?.classList.add('active');
        if (target === 2) document.getElementById('btn-chart-reviews')?.classList.add('active');

        growthChartInstance.data.datasets.forEach((ds, idx) => {
            ds.hidden = (idx !== target);
        });
    }
    growthChartInstance.update();
}

function initCategoryDonut() {
    const canvas = document.getElementById('categoryDonutCanvas');
    if (!canvas || typeof Chart === 'undefined') return;

    if (donutChartInstance) {
        donutChartInstance.destroy();
    }

    const ctx = canvas.getContext('2d');

    donutChartInstance = new Chart(ctx, {
        type: 'doughnut',
        data: {
            labels: ['Ẩm thực & Cà phê', 'Danh lam thắng cảnh', 'Vui chơi giải trí', 'Khách sạn & Lưu trú'],
            datasets: [{
                data: [42, 27, 18, 13],
                backgroundColor: ['#d9532f', '#0284c7', '#10b981', '#f59e0b'],
                borderWidth: 3,
                borderColor: '#ffffff',
                hoverOffset: 6
            }]
        },
        options: {
            responsive: true,
            maintainAspectRatio: false,
            cutout: '70%',
            plugins: {
                legend: { display: false },
                tooltip: {
                    callbacks: {
                        label: function(item) {
                            return ` ${item.label}: ${item.raw}%`;
                        }
                    }
                }
            }
        }
    });
}

function filterTopPlaces(type, btn) {
    document.querySelectorAll('.dash-card .time-pill-btn').forEach(b => b.classList.remove('active'));
    if (btn) btn.classList.add('active');
    renderTopPlaces(type);
}

function renderTopPlaces(type = 'rating') {
    const container = document.getElementById('top-places-list');
    if (!container) return;

    let sorted = [...mockTopPlacesData];
    if (type === 'reviews') {
        sorted.sort((a, b) => b.reviews - a.reviews);
    } else {
        sorted.sort((a, b) => b.rating - a.rating);
    }

    container.innerHTML = sorted.map((p, idx) => `
        <div class="top-place-item">
            <div class="top-rank-num ${idx === 0 ? 'top-rank-1' : idx === 1 ? 'top-rank-2' : idx === 2 ? 'top-rank-3' : ''}">
                ${idx + 1}
            </div>
            <img src="${p.image}" alt="${p.name}" class="top-place-img" onerror="this.src='https://images.unsplash.com/photo-1507525428034-b723cf961d3e?w=400'" />
            <div style="flex:1; min-width:0;">
                <div style="font-weight:700; font-size:0.88rem; color:var(--text-main); white-space:nowrap; overflow:hidden; text-overflow:ellipsis;">
                    ${p.name}
                </div>
                <small style="color:var(--text-muted); font-size:0.78rem;">${p.province}</small>
            </div>
            <div style="text-align:right; flex-shrink:0;">
                <div style="font-weight:800; font-size:0.88rem; color:var(--primary);">⭐ ${p.rating.toFixed(1)}</div>
                <small style="color:var(--text-muted); font-size:0.75rem;">${p.reviews.toLocaleString('vi-VN')} đánh giá</small>
            </div>
        </div>
    `).join('');
}

function renderRecentActivityStream() {
    const container = document.getElementById('recent-activity-stream');
    if (!container) return;

    container.innerHTML = fallbackAuditLogs.map(log => `
        <div class="activity-item">
            <div class="activity-dot"></div>
            <div class="activity-body">
                <div class="activity-text">
                    <strong>${log.userName}:</strong> ${log.action}
                </div>
                <div class="activity-time">${log.description} · ${getRelativeTime(log.createdAt)}</div>
            </div>
        </div>
    `).join('');
}

function getRelativeTime(dateStr) {
    const diffMs = Date.now() - new Date(dateStr).getTime();
    const mins = Math.floor(diffMs / 60000);
    if (mins < 1) return 'Vừa xong';
    if (mins < 60) return `${mins} phút trước`;
    const hours = Math.floor(mins / 60);
    if (hours < 24) return `${hours} giờ trước`;
    return `${Math.floor(hours / 24)} ngày trước`;
}

// --------------------------------------------------------------------------
// 4. SUBTAB TABLES & ACTIONS (PLACES, EDITS, REPORTS, APPEALS, AUDIT LOGS)
// --------------------------------------------------------------------------
async function loadPendingPlaces() {
    const tbody = document.getElementById('table-body');
    const thead = document.getElementById('table-head');
    
    thead.innerHTML = `
        <th>Tên địa điểm</th>
        <th>Danh mục</th>
        <th>Người đề xuất</th>
        <th>Ngày gửi</th>
        <th>Thao tác</th>
    `;
    tbody.innerHTML = '<tr><td colspan="5" style="text-align:center;">Đang tải...</td></tr>';

    let places = [];
    try {
        if (typeof api !== 'undefined') {
            places = await api.getPendingPlaces();
        }
    } catch (e) {
        places = fallbackPendingPlaces;
    }

    if (!places || places.length === 0) {
        places = fallbackPendingPlaces;
    }

    if (places.length === 0) {
        tbody.innerHTML = '<tr><td colspan="5" style="text-align:center; padding:30px; color:var(--text-muted);">Tất cả địa điểm đã được kiểm duyệt! Không có địa điểm nào chờ duyệt.</td></tr>';
        return;
    }

    tbody.innerHTML = places.map(p => `
        <tr>
            <td>
                <div style="font-weight:700; color:var(--text-main);">${p.name}</div>
                <small style="color:var(--text-muted);">${p.address || ''}</small>
            </td>
            <td><span class="badge badge-category">${p.categoryName || p.category || 'Địa điểm'}</span></td>
            <td>${p.proposer?.fullName || p.proposerName || 'Thành viên cộng đồng'}</td>
            <td>${new Date(p.createdAt || Date.now()).toLocaleDateString('vi-VN')}</td>
            <td>
                <div style="display:flex; gap:6px;">
                    <button class="action-btn btn-approve" onclick="approvePlace(${p.id})">Duyệt</button>
                    <button class="action-btn btn-reject" onclick="openRejectModal(${p.id}, 'place')">Từ chối</button>
                </div>
            </td>
        </tr>
    `).join('');
}

async function loadPendingEdits() {
    const tbody = document.getElementById('table-body');
    const thead = document.getElementById('table-head');
    
    thead.innerHTML = `
        <th>Địa điểm</th>
        <th>Người đề xuất</th>
        <th>Ngày gửi</th>
        <th>Nội dung thay đổi</th>
        <th>Thao tác</th>
    `;
    tbody.innerHTML = '<tr><td colspan="5" style="text-align:center;">Đang tải...</td></tr>';

    let edits = [];
    try {
        if (typeof api !== 'undefined') {
            edits = await api.getPendingEdits();
        }
    } catch (e) {
        edits = fallbackPendingEdits;
    }

    if (!edits || edits.length === 0) {
        edits = fallbackPendingEdits;
    }

    if (edits.length === 0) {
        tbody.innerHTML = '<tr><td colspan="5" style="text-align:center; padding:30px; color:var(--text-muted);">Không có đề xuất chỉnh sửa nào đang chờ duyệt.</td></tr>';
        return;
    }

    tbody.innerHTML = edits.map(e => `
        <tr>
            <td><strong>${e.placeName}</strong></td>
            <td>${e.proposerName || 'Thành viên'}</td>
            <td>${new Date(e.submittedAt).toLocaleDateString('vi-VN')}</td>
            <td>
                <button class="btn btn-outline btn-sm" onclick="alert('Chi tiết cập nhật:\\n' + JSON.stringify(e.proposedData, null, 2))">
                    Xem chi tiết
                </button>
            </td>
            <td>
                <div style="display:flex; gap:6px;">
                    <button class="action-btn btn-approve" onclick="approveEdit(${e.id})">Duyệt</button>
                    <button class="action-btn btn-reject" onclick="openRejectModal(${e.id}, 'edit')">Từ chối</button>
                </div>
            </td>
        </tr>
    `).join('');
}

async function loadReports() {
    const tbody = document.getElementById('table-body');
    const thead = document.getElementById('table-head');
    
    thead.innerHTML = `
        <th>Nội dung vi phạm</th>
        <th>Người báo cáo</th>
        <th>Ngày gửi</th>
        <th>Lý do báo cáo</th>
        <th>Thao tác</th>
    `;
    tbody.innerHTML = '<tr><td colspan="5" style="text-align:center;">Đang tải...</td></tr>';

    let reports = [];
    try {
        if (typeof api !== 'undefined') {
            reports = await api.getReports();
        }
    } catch (e) {
        reports = fallbackReports;
    }

    if (!reports || reports.length === 0) {
        reports = fallbackReports;
    }

    if (reports.length === 0) {
        tbody.innerHTML = '<tr><td colspan="5" style="text-align:center; padding:30px; color:var(--text-muted);">Không có báo cáo vi phạm nào chờ xử lý.</td></tr>';
        return;
    }

    tbody.innerHTML = reports.map(r => `
        <tr>
            <td><strong>${r.targetTitle}</strong></td>
            <td>${r.reporterName}</td>
            <td>${new Date(r.submittedAt).toLocaleDateString('vi-VN')}</td>
            <td><span style="color:var(--rose); font-weight:600;">${r.reasonContent}</span></td>
            <td>
                <div style="display:flex; gap:6px;">
                    <button class="action-btn btn-approve" onclick="resolveReport(${r.id}, true)">Xác nhận VP</button>
                    <button class="action-btn btn-reject" onclick="resolveReport(${r.id}, false)">Bỏ qua</button>
                </div>
            </td>
        </tr>
    `).join('');
}

async function resolveReport(id, confirmViolation) {
    if (!confirm(confirmViolation ? 'Xác nhận gỡ nội dung vi phạm này?' : 'Bỏ qua báo cáo này?')) return;
    try {
        if (typeof api !== 'undefined') {
            await api.resolveReport(id, confirmViolation, "Đã xử lý bởi Admin");
        }
    } catch (e) {}

    fallbackReports = fallbackReports.filter(r => r.id !== id);
    if (typeof UI !== 'undefined') {
        UI.showToast(confirmViolation ? 'Đã xử lý vi phạm thành công!' : 'Đã bỏ qua báo cáo', 'success');
    }
    loadReports();
    loadDashboard();
}

async function loadAppeals() {
    const tbody = document.getElementById('table-body');
    const thead = document.getElementById('table-head');
    
    thead.innerHTML = `
        <th>Đối tượng khiếu nại</th>
        <th>Người khiếu nại</th>
        <th>Ngày gửi</th>
        <th>Nội dung giải trình</th>
        <th>Thao tác</th>
    `;
    tbody.innerHTML = '<tr><td colspan="5" style="text-align:center;">Đang tải...</td></tr>';

    let appeals = [];
    try {
        if (typeof api !== 'undefined') {
            appeals = await api.getAppeals();
        }
    } catch (e) {
        appeals = fallbackAppeals;
    }

    if (!appeals || appeals.length === 0) {
        appeals = fallbackAppeals;
    }

    if (appeals.length === 0) {
        tbody.innerHTML = '<tr><td colspan="5" style="text-align:center; padding:30px; color:var(--text-muted);">Không có khiếu nại nào chờ xử lý.</td></tr>';
        return;
    }

    tbody.innerHTML = appeals.map(a => `
        <tr>
            <td><strong>${a.targetType} #${a.targetId}</strong></td>
            <td>${a.userName}</td>
            <td>${new Date(a.submittedAt).toLocaleDateString('vi-VN')}</td>
            <td>${a.reason}</td>
            <td>
                <div style="display:flex; gap:6px;">
                    <button class="action-btn btn-approve" onclick="handleAppeal(${a.id}, 'Đã giải quyết')">Phê duyệt</button>
                    <button class="action-btn btn-reject" onclick="handleAppeal(${a.id}, 'Từ chối khiếu nại')">Từ chối</button>
                </div>
            </td>
        </tr>
    `).join('');
}

async function handleAppeal(id, result, escalate = false) {
    if (!confirm(`Xác nhận xử lý khiếu nại: "${result}"?`)) return;
    try {
        if (typeof api !== 'undefined') {
            await api.handleAppeal(id, result, escalate);
        }
    } catch (e) {}

    fallbackAppeals = fallbackAppeals.filter(a => a.id !== id);
    if (typeof UI !== 'undefined') {
        UI.showToast(`Đã xử lý khiếu nại: ${result}`, 'success');
    }
    loadAppeals();
}

async function loadAuditLogs() {
    const tbody = document.getElementById('table-body');
    const thead = document.getElementById('table-head');
    
    thead.innerHTML = `
        <th>Quản trị viên</th>
        <th>Hành động</th>
        <th>Đối tượng</th>
        <th>Mô tả chi tiết</th>
        <th>Thời gian</th>
    `;
    tbody.innerHTML = '<tr><td colspan="5" style="text-align:center;">Đang tải...</td></tr>';

    let logs = [];
    try {
        if (typeof api !== 'undefined') {
            logs = await api.getAuditLogs();
        }
    } catch (e) {
        logs = fallbackAuditLogs;
    }

    if (!logs || logs.length === 0) {
        logs = fallbackAuditLogs;
    }

    tbody.innerHTML = logs.map(l => `
        <tr>
            <td><strong>${l.userName}</strong></td>
            <td><span class="status-badge" style="background:#f1f5f9; color:#334155; font-weight:700;">${l.action}</span></td>
            <td>${l.targetType || ''} #${l.targetId || ''}</td>
            <td>${l.description || ''}</td>
            <td>${new Date(l.createdAt).toLocaleString('vi-VN')}</td>
        </tr>
    `).join('');
}

async function approvePlace(id) {
    const place = fallbackPendingPlaces.find(p => p.id === id);
    const placeName = place ? place.name : 'Địa điểm mới';

    if (!confirm(`Xác nhận phê duyệt địa điểm "${placeName}"?`)) return;
    
    try {
        if (typeof api !== 'undefined') {
            await api.approvePlace(id);
        }
    } catch (e) {}

    if (typeof TravelData !== 'undefined') {
        TravelData.addNotification({
            title: 'Địa điểm được phê duyệt',
            content: `Địa điểm "${placeName}" bạn đề xuất đã được Ban quản trị duyệt và hiển thị công khai!`,
            type: 'approved',
            time: 'Vừa xong',
            targetUrl: 'search.html'
        });
        if (typeof UI !== 'undefined') UI.updateNotificationBadge();
    }

    fallbackPendingPlaces = fallbackPendingPlaces.filter(p => p.id !== id);
    if (typeof UI !== 'undefined') {
        UI.showToast(`Đã duyệt thành công địa điểm "${placeName}"!`, 'success');
    }
    loadPendingPlaces();
    loadDashboard();
}

async function approveEdit(id) {
    if (!confirm('Xác nhận phê duyệt đề xuất chỉnh sửa này?')) return;
    try {
        if (typeof api !== 'undefined') {
            await api.approveEdit(id);
        }
    } catch (e) {}

    fallbackPendingEdits = fallbackPendingEdits.filter(e => e.id !== id);
    if (typeof UI !== 'undefined') {
        UI.showToast('Đã duyệt đề xuất chỉnh sửa thành công!', 'success');
    }
    loadPendingEdits();
}

function openRejectModal(id, type) {
    currentRejectId = id;
    currentRejectType = type;
    document.getElementById('reject-reason').value = '';
    document.getElementById('reject-modal').classList.add('show');
}

function closeModal(id) {
    document.getElementById(id).classList.remove('show');
}

async function submitReject() {
    const reason = document.getElementById('reject-reason').value.trim();
    if (!reason) {
        alert('Vui lòng nhập lý do từ chối.');
        return;
    }

    try {
        if (currentRejectType === 'place') {
            try {
                if (typeof api !== 'undefined') await api.rejectPlace(currentRejectId, reason);
            } catch (e) {}
            
            const place = fallbackPendingPlaces.find(p => p.id === currentRejectId);
            const placeName = place ? place.name : 'Địa điểm';

            if (typeof TravelData !== 'undefined') {
                TravelData.addNotification({
                    title: 'Đề xuất địa điểm bị từ chối',
                    content: `Địa điểm "${placeName}" chưa được duyệt. Lý do: ${reason}`,
                    type: 'warning',
                    time: 'Vừa xong',
                    targetUrl: 'propose-place.html'
                });
                if (typeof UI !== 'undefined') UI.updateNotificationBadge();
            }

            fallbackPendingPlaces = fallbackPendingPlaces.filter(p => p.id !== currentRejectId);
            if (typeof UI !== 'undefined') UI.showToast('Đã từ chối địa điểm.', 'info');
            loadPendingPlaces();
        } else if (currentRejectType === 'edit') {
            fallbackPendingEdits = fallbackPendingEdits.filter(e => e.id !== currentRejectId);
            if (typeof UI !== 'undefined') UI.showToast('Đã từ chối đề xuất chỉnh sửa.', 'info');
            loadPendingEdits();
        }
        loadDashboard();
        closeModal('reject-modal');
    } catch (e) {
        alert('Lỗi: ' + e.message);
    }
}
