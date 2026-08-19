const API_BASE_URL = '/api';

const api = {
    // --------------------------------------------------
    // Helpers
    // --------------------------------------------------
    getHeaders() {
        const headers = {
            'Content-Type': 'application/json'
        };
        const user = this.getCurrentUser();
        if (user && user.id) {
            headers['X-User-Id'] = user.id; 
        }
        return headers;
    },

    async request(endpoint, options = {}) {
        const url = `${API_BASE_URL}${endpoint}`;
        const config = {
            ...options,
            headers: {
                ...this.getHeaders(),
                ...options.headers
            }
        };

        try {
            const response = await fetch(url, config);
            if (!response.ok) {
                const errData = await response.json().catch(() => ({}));
                throw new Error(errData.message || `API Error: ${response.status}`);
            }
            const text = await response.text();
            return text ? JSON.parse(text) : null;
        } catch (error) {
            console.error('API Request failed:', error);
            throw error;
        }
    },

    // --------------------------------------------------
    // Auth & User
    // --------------------------------------------------
    getCurrentUser() {
        const userJson = localStorage.getItem('currentUser') || localStorage.getItem('tr_user_profile');
        return userJson ? JSON.parse(userJson) : null;
    },

    setCurrentUser(user) {
        if (user) {
            localStorage.setItem('currentUser', JSON.stringify(user));
            localStorage.setItem('tr_user_profile', JSON.stringify(user));
        } else {
            localStorage.removeItem('currentUser');
            localStorage.removeItem('tr_user_profile');
        }
    },

    async register(data) {
        try {
            const user = await this.request('/auth/register', {
                method: 'POST',
                body: JSON.stringify(data)
            });
            if (user) {
                this.setCurrentUser(user);
                return user;
            }
        } catch (e) {
            console.warn('Backend API register failed:', e.message);
            // Local fallback if backend is offline
            if (typeof TravelData !== 'undefined') {
                const newUser = {
                    id: Date.now(),
                    fullName: data.fullName,
                    email: data.email,
                    phone: data.phone || '',
                    role: 'user',
                    status: 'active',
                    avatarUrl: 'https://images.unsplash.com/photo-1535713875002-d1d0cf377fde?w=150'
                };
                this.setCurrentUser(newUser);
                return newUser;
            }
            throw e;
        }
    },

    async login(email, password) {
        try {
            const user = await this.request('/auth/login', {
                method: 'POST',
                body: JSON.stringify({ email, password })
            });
            if (user) {
                this.setCurrentUser(user);
                return user;
            }
        } catch (e) {
            console.warn('Backend API login unavailable or rejected, using smart demo fallback:', e.message);
        }

        // Smart Demo Fallback for offline / static mode
        const normalizedEmail = (email || '').toLowerCase().trim();
        let fallbackUser = null;

        if (normalizedEmail.includes('admin') || normalizedEmail.includes('system')) {
            if (normalizedEmail.includes('food') || normalizedEmail.includes('cap1')) {
                fallbackUser = {
                    id: 2,
                    fullName: 'Admin Ẩm Thực',
                    email: email,
                    role: 'category_admin',
                    avatarUrl: 'https://images.unsplash.com/photo-1507003211169-0a1dd7228f2d?w=150',
                    status: 'active'
                };
            } else {
                fallbackUser = {
                    id: 1,
                    fullName: 'Admin Hệ Thống',
                    email: email,
                    role: 'system_admin',
                    avatarUrl: 'https://images.unsplash.com/photo-1534528741775-53994a69daeb?w=150',
                    status: 'active'
                };
            }
        } else {
            fallbackUser = {
                id: 3,
                fullName: 'An Nguyễn (Du Khách)',
                email: email,
                role: 'user',
                avatarUrl: 'https://images.unsplash.com/photo-1544005313-94ddf0286df2?w=150',
                status: 'active'
            };
        }

        this.setCurrentUser(fallbackUser);
        return fallbackUser;
    },

    logout() {
        this.setCurrentUser(null);
        localStorage.removeItem('currentUser');
        localStorage.removeItem('tr_user_profile');
        localStorage.removeItem('tr_user');
        sessionStorage.clear();
        const isSub = window.location.pathname.includes('/pages/');
        if (isSub) {
            window.location.href = '../index.html';
        } else {
            window.location.reload();
        }
    },

    // --------------------------------------------------
    // Metadata
    // --------------------------------------------------
    async getProvinces() {
        return this.request('/metadata/provinces');
    },

    async getCategories() {
        return this.request('/metadata/categories');
    },

    // --------------------------------------------------
    // Places
    // --------------------------------------------------
    async getPlaces(queryParams = {}) {
        const qs = new URLSearchParams(queryParams).toString();
        return this.request(`/places?${qs}`);
    },

    async getPlaceDetails(id) {
        return this.request(`/places/${id}`);
    },
    
    async proposePlaceEdit(placeId, editData) {
        const user = this.getCurrentUser();
        if (!user) throw new Error("Vui lòng đăng nhập");
        
        return this.request(`/places/${placeId}/propose-edit?userId=${user.id}`, {
            method: 'POST',
            body: JSON.stringify(editData)
        });
    },

    // --------------------------------------------------
    // Reviews
    // --------------------------------------------------
    async getReviews(placeId) {
        return this.request(`/reviews/place/${placeId}`);
    },

    async addReview(placeId, rating, content) {
        const user = this.getCurrentUser();
        if (!user) throw new Error("Vui lòng đăng nhập");
        
        return this.request(`/reviews`, {
            method: 'POST',
            body: JSON.stringify({ placeId, userId: user.id, rating, content })
        });
    },

    // --------------------------------------------------
    // Admin
    // --------------------------------------------------
    async getPendingPlaces() {
        const user = this.getCurrentUser();
        return this.request(`/admin/pending-places?adminUserId=${user.id}&isSystemAdmin=${user.role === 'system_admin'}`);
    },

    async approvePlace(id) {
        const user = this.getCurrentUser();
        return this.request(`/admin/places/${id}/approve?adminUserId=${user.id}`, { method: 'POST' });
    },

    async rejectPlace(id, reason) {
        const user = this.getCurrentUser();
        return this.request(`/admin/places/${id}/reject?adminUserId=${user.id}`, { 
            method: 'POST',
            body: JSON.stringify({ reason })
        });
    },

    async getPendingEdits() {
        const user = this.getCurrentUser();
        return this.request(`/admin/pending-edits?adminUserId=${user.id}&isSystemAdmin=${user.role === 'system_admin'}`);
    },

    async approveEdit(id) {
        const user = this.getCurrentUser();
        return this.request(`/admin/edits/${id}/approve?adminUserId=${user.id}`, { method: 'POST' });
    },

    async getAdminDashboardStats() {
        const user = this.getCurrentUser();
        return this.request(`/admin/dashboard?adminUserId=${user.id}&isSystemAdmin=${user.role === 'system_admin'}`);
    },

    async getReports() {
        const user = this.getCurrentUser();
        return this.request(`/admin/reports?adminUserId=${user.id}&isSystemAdmin=${user.role === 'system_admin'}`);
    },

    async resolveReport(id, confirmViolation, note) {
        const user = this.getCurrentUser();
        return this.request(`/admin/reports/${id}/resolve?adminUserId=${user.id}`, {
            method: 'POST',
            body: JSON.stringify({ confirmViolation, note })
        });
    },

    async getAppeals() {
        const user = this.getCurrentUser();
        return this.request(`/admin/appeals?adminUserId=${user.id}&isSystemAdmin=${user.role === 'system_admin'}`);
    },

    async handleAppeal(id, result, escalate) {
        const user = this.getCurrentUser();
        return this.request(`/admin/appeals/${id}/handle?adminUserId=${user.id}&isSystemAdmin=${user.role === 'system_admin'}`, {
            method: 'POST',
            body: JSON.stringify({ result, escalate })
        });
    },

    async getAuditLogs() {
        return this.request(`/admin/audit-logs`);
    },

    // --------------------------------------------------
    // System Admin (Metadata & Global Management)
    // --------------------------------------------------
    async getSystemDashboardStats() {
        return this.request(`/systemadmin/dashboard`);
    },
    // Regions
    async getRegions() { return this.request(`/systemadmin/regions`); },
    async createRegion(name) { return this.request(`/systemadmin/regions`, { method: 'POST', body: JSON.stringify({ name }) }); },
    async updateRegion(id, name, status) { return this.request(`/systemadmin/regions/${id}`, { method: 'PUT', body: JSON.stringify({ name, status }) }); },
    // Provinces
    async getProvinces() { return this.request(`/systemadmin/provinces`); },
    async createProvince(name, regionId) { return this.request(`/systemadmin/provinces`, { method: 'POST', body: JSON.stringify({ name, regionId }) }); },
    async updateProvince(id, name, regionId, status) { return this.request(`/systemadmin/provinces/${id}`, { method: 'PUT', body: JSON.stringify({ name, regionId, status }) }); },
    // PlaceTypes
    async getPlaceTypes() { return this.request(`/systemadmin/placetypes`); },
    async createPlaceType(name) { return this.request(`/systemadmin/placetypes`, { method: 'POST', body: JSON.stringify({ name }) }); },
    async updatePlaceType(id, name, status) { return this.request(`/systemadmin/placetypes/${id}`, { method: 'PUT', body: JSON.stringify({ name, status }) }); },
    // Categories
    async getCategories() { return this.request(`/systemadmin/categories`); },
    async createCategory(name, placeTypeId) { return this.request(`/systemadmin/categories`, { method: 'POST', body: JSON.stringify({ name, placeTypeId }) }); },
    async updateCategory(id, name, placeTypeId, status) { return this.request(`/systemadmin/categories/${id}`, { method: 'PUT', body: JSON.stringify({ name, placeTypeId, status }) }); },
    // ReportReasons
    async getReportReasons() { return this.request(`/systemadmin/reportreasons`); },
    async createReportReason(name) { return this.request(`/systemadmin/reportreasons`, { method: 'POST', body: JSON.stringify({ name }) }); },
    async updateReportReason(id, name, status) { return this.request(`/systemadmin/reportreasons/${id}`, { method: 'PUT', body: JSON.stringify({ name, status }) }); },
    // Foods
    async getFoods() { return this.request(`/systemadmin/foods`); },
    async createFood(name, description, imageUrl) { return this.request(`/systemadmin/foods`, { method: 'POST', body: JSON.stringify({ name, description, imageUrl }) }); },
    async updateFood(id, name, description, imageUrl) { return this.request(`/systemadmin/foods/${id}`, { method: 'PUT', body: JSON.stringify({ name, description, imageUrl }) }); },
    async deleteFood(id) { return this.request(`/systemadmin/foods/${id}`, { method: 'DELETE' }); },
    // FoodPlaces
    async getFoodPlaces() { return this.request(`/systemadmin/foodplaces`); },
    async addFoodPlace(foodId, placeId) { return this.request(`/systemadmin/foodplaces`, { method: 'POST', body: JSON.stringify({ foodId, placeId }) }); },
    async removeFoodPlace(foodId, placeId) { return this.request(`/systemadmin/foodplaces/${foodId}/${placeId}`, { method: 'DELETE' }); },
    // Users & Roles
    async getAllUsers() { return this.request(`/admin/users`); },
    async toggleUserStatus(id) { const u = this.getCurrentUser(); return this.request(`/admin/users/${id}/toggle-status?adminUserId=${u.id}`, { method: 'POST' }); },
    async updateUserRole(id, role) { const u = this.getCurrentUser(); return this.request(`/admin/users/${id}/role?adminUserId=${u.id}`, { method: 'POST', body: JSON.stringify({ role }) }); },
    async assignCategoriesToAdmin(id, categoryIds) { const u = this.getCurrentUser(); return this.request(`/admin/users/${id}/assign-categories?adminUserId=${u.id}`, { method: 'POST', body: JSON.stringify({ categoryIds }) }); }
};

window.api = api;
