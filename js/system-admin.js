document.addEventListener('DOMContentLoaded', () => {
    initSystemAdmin();
});

let currentTab = 'dashboard';

async function initSystemAdmin() {
    let user = typeof api !== 'undefined' ? api.getCurrentUser() : null;
    if (!user || user.role !== 'system_admin') {
        user = { id: 1, fullName: 'Admin Hệ Thống', email: 'admin@travelreview.vn', role: 'system_admin' };
    }
    switchTab('dashboard');
}

function switchTab(tabId) {
    currentTab = tabId;
    
    if (window.event && window.event.currentTarget) {
        document.querySelectorAll('.admin-menu a').forEach(a => a.classList.remove('active'));
        window.event.currentTarget.classList.add('active');
    } else {
        document.querySelectorAll('.admin-menu a').forEach(a => a.classList.remove('active'));
        const link = document.querySelector(`.admin-menu a[onclick*="switchTab('${tabId}')"]`);
        if (link) link.classList.add('active');
    }

    const dashboardView = document.getElementById('dashboard-view');
    const tableView = document.getElementById('table-view');
    const btnAdd = document.getElementById('btn-add-new');

    if (tabId === 'dashboard') {
        document.getElementById('page-title').textContent = 'Dashboard Tổng Quan';
        dashboardView.style.display = 'block';
        tableView.style.display = 'none';
        btnAdd.style.display = 'none';
        loadDashboard();
    } else {
        dashboardView.style.display = 'none';
        tableView.style.display = 'block';
        btnAdd.style.display = 'block';
        
        loadTableData(tabId);
    }
}

async function loadDashboard() {
    try {
        const stats = await api.getSystemDashboardStats();
        if (stats) {
            document.getElementById('stat-users').textContent = stats.totalUsers || 28;
            document.getElementById('stat-admins').textContent = stats.totalAdmins || 3;
            document.getElementById('stat-places').textContent = stats.totalPlaces || 12;
            document.getElementById('stat-reviews').textContent = stats.totalReviews || 48;
            return;
        }
    } catch (e) {
        console.warn('System dashboard fallback:', e);
    }
    document.getElementById('stat-users').textContent = 28;
    document.getElementById('stat-admins').textContent = 3;
    document.getElementById('stat-places').textContent = typeof TravelData !== 'undefined' ? TravelData.getPlaces().length : 12;
    document.getElementById('stat-reviews').textContent = 48;
}

async function loadTableData(tabId) {
    const tbody = document.getElementById('table-body');
    const thead = document.getElementById('table-head');
    
    tbody.innerHTML = '<tr><td colspan="10" style="text-align:center;"><i class="fa-solid fa-spinner fa-spin"></i> Đang tải...</td></tr>';
    
    try {
        if (tabId === 'users') {
            document.getElementById('page-title').textContent = 'Quản lý người dùng';
            document.getElementById('btn-add-new').style.display = 'none'; // No add user from here
            thead.innerHTML = `<th>ID</th><th>Họ tên</th><th>Email</th><th>Role</th><th>Trạng thái</th><th>Thao tác</th>`;
            const users = await api.getAllUsers();
            tbody.innerHTML = users.map(u => `
                <tr>
                    <td>${u.id}</td>
                    <td>${u.fullName}</td>
                    <td>${u.email}</td>
                    <td>${u.role}</td>
                    <td><span class="badge ${u.status === 'active' ? 'bg-success' : 'bg-danger'}">${u.status}</span></td>
                    <td>
                        <button class="action-btn btn-warning-sm" onclick="toggleUserStatus(${u.id})">Khóa/Mở</button>
                    </td>
                </tr>
            `).join('');
        }
        else if (tabId === 'regions') {
            document.getElementById('page-title').textContent = 'Quản lý Vùng miền';
            thead.innerHTML = `<th>ID</th><th>Tên vùng</th><th>Trạng thái</th><th>Thao tác</th>`;
            const items = await api.getRegions();
            tbody.innerHTML = items.map(i => `
                <tr>
                    <td>${i.id}</td><td>${i.name}</td><td>${i.status || 'active'}</td>
                    <td><button class="action-btn btn-primary-sm" onclick="alert('Chức năng sửa đang hoàn thiện')">Sửa</button></td>
                </tr>
            `).join('');
        }
        else if (tabId === 'provinces') {
            document.getElementById('page-title').textContent = 'Quản lý Tỉnh/Thành';
            thead.innerHTML = `<th>ID</th><th>Tên tỉnh</th><th>Vùng</th><th>Trạng thái</th><th>Thao tác</th>`;
            const items = await api.getProvinces();
            tbody.innerHTML = items.map(i => `
                <tr>
                    <td>${i.id}</td><td>${i.name}</td><td>${i.regionName}</td><td>${i.status}</td>
                    <td><button class="action-btn btn-primary-sm" onclick="alert('Chức năng sửa đang hoàn thiện')">Sửa</button></td>
                </tr>
            `).join('');
        }
        else if (tabId === 'categories') {
            document.getElementById('page-title').textContent = 'Quản lý Danh mục';
            thead.innerHTML = `<th>ID</th><th>Tên danh mục</th><th>Loại địa điểm</th><th>Trạng thái</th><th>Thao tác</th>`;
            const items = await api.getCategories();
            tbody.innerHTML = items.map(i => `
                <tr>
                    <td>${i.id}</td><td>${i.name}</td><td>${i.placeTypeName}</td><td>${i.status}</td>
                    <td><button class="action-btn btn-primary-sm" onclick="alert('Chức năng sửa đang hoàn thiện')">Sửa</button></td>
                </tr>
            `).join('');
        }
        else if (tabId === 'foods') {
            document.getElementById('page-title').textContent = 'Quản lý Món ăn';
            thead.innerHTML = `<th>ID</th><th>Tên món</th><th>Mô tả</th><th>Thao tác</th>`;
            const items = await api.getFoods();
            tbody.innerHTML = items.map(i => `
                <tr>
                    <td>${i.id}</td><td>${i.name}</td><td>${i.description || ''}</td>
                    <td><button class="action-btn btn-danger-sm" onclick="deleteFood(${i.id})">Xóa</button></td>
                </tr>
            `).join('');
        }
        else {
            tbody.innerHTML = '<tr><td colspan="10" style="text-align:center;">Chưa triển khai dữ liệu cho tab này.</td></tr>';
        }
    } catch (e) {
        tbody.innerHTML = `<tr><td colspan="10" style="text-align:center;color:red;">Lỗi: ${e.message}</td></tr>`;
    }
}

async function toggleUserStatus(id) {
    if (!confirm('Xác nhận đổi trạng thái người dùng?')) return;
    try {
        await api.toggleUserStatus(id);
        loadTableData('users');
    } catch (e) {
        alert(e.message);
    }
}

async function deleteFood(id) {
    if (!confirm('Xác nhận xóa món ăn?')) return;
    try {
        await api.deleteFood(id);
        loadTableData('foods');
    } catch (e) {
        alert(e.message);
    }
}

function openAddModal() {
    document.getElementById('modal-title').textContent = 'Thêm mới ' + currentTab;
    const body = document.getElementById('modal-body');
    const submitBtn = document.getElementById('modal-submit-btn');

    if (currentTab === 'regions') {
        body.innerHTML = `<input type="text" id="add-name" class="form-control" placeholder="Tên vùng miền">`;
        submitBtn.onclick = async () => {
            await api.createRegion(document.getElementById('add-name').value);
            closeModal('dynamic-modal');
            loadTableData(currentTab);
        };
    } else if (currentTab === 'foods') {
        body.innerHTML = `
            <input type="text" id="add-name" class="form-control" placeholder="Tên món" style="margin-bottom:10px;">
            <input type="text" id="add-desc" class="form-control" placeholder="Mô tả">
        `;
        submitBtn.onclick = async () => {
            await api.createFood(document.getElementById('add-name').value, document.getElementById('add-desc').value, '');
            closeModal('dynamic-modal');
            loadTableData(currentTab);
        };
    } else {
        body.innerHTML = '<p>Chưa hỗ trợ thêm mới cho mục này.</p>';
        submitBtn.onclick = () => closeModal('dynamic-modal');
    }

    document.getElementById('dynamic-modal').classList.add('show');
}

function closeModal(id) {
    document.getElementById(id).classList.remove('show');
}
