/* ==========================================================================
   MINIMAP & LANG THANG — UI UTILITIES & COMMON COMPONENTS
   Warm Terracotta Palette + Multi-Level Menus & Profile Linking
   ========================================================================== */

const UI = (() => {
  const isSubPage = window.location.pathname.includes('/pages/');
  const rootPath = isSubPage ? '../' : './';
  const pagesPath = isSubPage ? './' : 'pages/';

  // --------------------------------------------------------------------------
  // 1. 5-BUBBLE RATING RENDERER (Warm Terracotta / Amber Palette)
  // --------------------------------------------------------------------------
  function renderBubbleRating(rating = 5.0, reviewCount = null, showScore = true, size = '') {
    const num = Math.min(5, Math.max(0, Number(rating) || 5.0));
    let bubblesHtml = '<div class="trip-bubbles ' + (size === 'lg' ? 'trip-bubbles-lg' : '') + '">';

    for (let i = 1; i <= 5; i++) {
      if (num >= i) {
        bubblesHtml += '<span class="trip-bubble"></span>';
      } else if (num >= i - 0.5) {
        bubblesHtml += '<span class="trip-bubble half"></span>';
      } else {
        bubblesHtml += '<span class="trip-bubble empty"></span>';
      }
    }
    bubblesHtml += '</div>';

    return `
      <div class="trip-bubble-rating">
        ${bubblesHtml}
        ${showScore ? `<span class="trip-rating-score">${num.toFixed(1)}</span>` : ''}
        ${reviewCount !== null ? `<span class="trip-review-count">(${Number(reviewCount).toLocaleString('vi-VN')})</span>` : ''}
      </div>
    `;
  }

  // --------------------------------------------------------------------------
  // 2. TRAVELERS' CHOICE & RANK BADGES
  // --------------------------------------------------------------------------
  function renderTravelersChoiceBadge(rankText = "Travelers' Choice 2024") {
    return `
      <span class="badge badge-travelers-choice" title="Đạt giải thưởng do cộng đồng du khách bình chọn">
        <i class="fa-solid fa-award"></i> ${rankText}
      </span>
    `;
  }

  // --------------------------------------------------------------------------
  // 3. HEADER COMPONENT WITH MULTI-LEVEL MEGA MENU & POPUPS
  // --------------------------------------------------------------------------
  function renderHeader(activePage = '') {
    // Get current user from API session or TravelData
    const user = typeof api !== 'undefined' ? api.getCurrentUser() : (typeof TravelData !== 'undefined' ? TravelData.getUser() : null);
    const initials = user && user.fullName ? user.fullName.split(' ').map(n => n[0]).slice(-2).join('').toUpperCase() : 'LT';
    const notis = typeof TravelData !== 'undefined' ? TravelData.getNotifications() : [];
    const hasUnread = notis.some(n => !n.isRead);

    const authArea = user
      ? `<div class="header-user-dropdown" id="headerUserDropdown">
           <button class="header-user-btn" onclick="UI.toggleUserDropdown(event)">
             ${user.avatarUrl
               ? `<img src="${user.avatarUrl}" alt="${user.fullName}" class="header-avatar-img">`
               : `<div class="user-profile-badge" title="${user.fullName}">${initials}</div>`
             }
             <span class="header-user-name" style="font-weight:700; font-size:0.9rem; color:var(--text-main);">${(user.fullName || 'Người dùng').split(' ').slice(-1)[0]}</span>
             <i class="fa-solid fa-chevron-down" style="font-size:0.7rem; color:var(--text-muted);"></i>
           </button>
           <div class="header-dropdown-menu" id="headerDropdownMenu">
             <a href="${pagesPath}profile.html?id=${user.id || 1}" class="dropdown-item"><i class="fa-regular fa-user"></i> Hồ sơ cá nhân</a>
             <a href="${pagesPath}my-proposals.html" class="dropdown-item"><i class="fa-solid fa-file-lines"></i> Đề xuất & Đóng góp</a>
             <a href="${pagesPath}visited.html" class="dropdown-item"><i class="fa-regular fa-calendar-check"></i> Nhật ký chuyến đi</a>
             <a href="${pagesPath}favorites.html" class="dropdown-item"><i class="fa-regular fa-heart"></i> Địa điểm yêu thích</a>
             <a href="${pagesPath}history.html" class="dropdown-item"><i class="fa-solid fa-clock-rotate-left"></i> Đã xem gần đây</a>
             ${(user.role === 'system_admin' || user.role === 'category_admin' || (user.role && user.role.includes('admin')))
               ? `<a href="${pagesPath}admin.html" class="dropdown-item" style="color:var(--primary); font-weight:700;"><i class="fa-solid fa-shield-halved"></i> Trang quản trị</a>` : ''}
             <div class="dropdown-divider"></div>
             <a href="${pagesPath}settings.html" class="dropdown-item"><i class="fa-solid fa-gear"></i> Cài đặt</a>
             <a href="javascript:void(0)" class="dropdown-item dropdown-logout" onclick="UI.handleLogout(event); return false;"><i class="fa-solid fa-right-from-bracket"></i> Đăng xuất</a>
           </div>
         </div>`
      : `<a href="${pagesPath}login.html" style="font-size:0.9rem; font-weight:700; color:var(--text-main); text-decoration:none; padding:8px 12px; border-radius:var(--radius-full); transition:background 0.2s;" onmouseover="this.style.background='var(--bg-subtle)'" onmouseout="this.style.background='transparent'">Đăng nhập</a>
         <a href="${pagesPath}login.html?tab=register" class="btn btn-primary btn-sm" style="font-size:0.88rem; font-weight:700; border-radius:var(--radius-full); padding:8px 20px;">Đăng ký</a>`;

    return `
      <div class="container">
        <div class="header-inner">
          <!-- Brand Logo -->
          <a href="${rootPath}index.html" class="brand-logo">
            <div class="logo-badge"><i class="fa-solid fa-compass"></i></div>
            <div class="brand-name">
              <span class="brand-name-main">Lang Thang</span>
              <span class="brand-name-sub">Cẩm nang du lịch Việt</span>
            </div>
          </a>

          <!-- Desktop Nav Links (Clean, modern typography without icon clutter) -->
          <nav class="nav-menu">
            <a href="${rootPath}index.html" class="nav-link ${activePage === 'home' ? 'active' : ''}">
              Trang chủ
            </a>

            <!-- Dropdown for 'Khám phá' -->
            <div class="nav-item-dropdown">
              <a href="${pagesPath}search.html" class="nav-link ${activePage === 'search' ? 'active' : ''}">
                Khám phá <i class="fa-solid fa-chevron-down" style="font-size:0.65rem; margin-left:4px; opacity:0.7;"></i>
              </a>
              <div class="mega-dropdown-menu">
                <div class="mega-menu-grid">
                  <!-- Col 1: Vùng miền -->
                  <div class="mega-menu-col">
                    <h5>Điểm đến</h5>
                    <ul class="mega-menu-links">
                      <li><a href="${pagesPath}search.html?province=TP.+Hồ+Chí+Minh" class="mega-menu-link">TP. Hồ Chí Minh</a></li>
                      <li><a href="${pagesPath}search.html?province=Hà+Nội" class="mega-menu-link">Hà Nội</a></li>
                      <li><a href="${pagesPath}search.html?province=Đà+Nẵng" class="mega-menu-link">Đà Nẵng & Hội An</a></li>
                      <li><a href="${pagesPath}search.html?province=Lâm+Đồng" class="mega-menu-link">Đà Lạt (Lâm Đồng)</a></li>
                      <li><a href="${pagesPath}search.html?province=Kiên+Giang" class="mega-menu-link">Phú Quốc</a></li>
                    </ul>
                  </div>

                  <!-- Col 2: Loại hình -->
                  <div class="mega-menu-col">
                    <h5>Trải nghiệm</h5>
                    <ul class="mega-menu-links">
                      <li><a href="${pagesPath}search.html?type=1" class="mega-menu-link">Ăn uống & Cà phê</a></li>
                      <li><a href="${pagesPath}search.html?type=2" class="mega-menu-link">Danh lam thắng cảnh</a></li>
                      <li><a href="${pagesPath}search.html?type=3" class="mega-menu-link">Khách sạn & Nghỉ dưỡng</a></li>
                      <li><a href="${pagesPath}search.html?type=4" class="mega-menu-link">Vui chơi & Giải trí</a></li>
                    </ul>
                  </div>

                  <!-- Col 3: Bộ sưu tập tuyển chọn -->
                  <div class="mega-menu-col" style="background:var(--bg-subtle); padding:14px 16px; border-radius:var(--radius-lg);">
                    <h5 style="color:var(--text-main);">Bộ sưu tập nổi bật</h5>
                    <ul class="mega-menu-links">
                      <li><a href="${pagesPath}ranking.html" class="mega-menu-link" style="font-weight:700; color:var(--primary);">Travelers' Choice 2024</a></li>
                      <li><a href="${pagesPath}foods.html" class="mega-menu-link">Món ăn đặc sản 3 miền</a></li>
                      <li><a href="${pagesPath}search.html?minRating=4.5" class="mega-menu-link">Điểm đến đánh giá 5.0</a></li>
                      <li><a href="${pagesPath}map.html" class="mega-menu-link">Bản đồ du lịch</a></li>
                    </ul>
                  </div>
                </div>
              </div>
            </div>

            <a href="${pagesPath}foods.html" class="nav-link ${activePage === 'foods' ? 'active' : ''}">
              Ẩm thực
            </a>
            <a href="${pagesPath}ranking.html" class="nav-link ${activePage === 'ranking' ? 'active' : ''}">
              Bảng xếp hạng
            </a>
            <a href="${pagesPath}map.html" class="nav-link ${activePage === 'map' ? 'active' : ''}">
              Bản đồ
            </a>
          </nav>

          <!-- Actions -->
          <div class="header-actions">
            <div class="notification-container" style="position: relative; display: inline-block;">
              <button class="header-btn-icon" id="notiBellBtn" title="Thông báo" onclick="UI.toggleNotificationDropdown(event)">
                <i class="fa-regular fa-bell" style="font-size:1.05rem;"></i>
                <span class="unread-dot" id="header-unread-dot" style="${hasUnread ? 'display:block;' : 'display:none;'}"></span>
              </button>

              <!-- Notification Popover -->
              <div id="headerNotificationDropdown" class="noti-panel" style="display:none;">
                <div class="noti-panel-header">
                  <div class="noti-panel-title">
                    <span>Thông báo</span>
                  </div>
                  <div style="display:flex; align-items:center; gap:8px;">
                    <button class="noti-mark-all-btn" onclick="UI.markAllNotiRead(event)">Đã đọc hết</button>
                    <a href="${pagesPath}notifications.html" class="noti-see-all">Xem tất cả</a>
                  </div>
                </div>
                <div id="headerNotificationList" class="noti-panel-list">
                  <div class="noti-panel-empty">Đang tải...</div>
                </div>
              </div>
            </div>

            <a href="${pagesPath}propose-place.html" class="btn-post-nav">
              Đóng góp
            </a>
            ${authArea}
            <button class="hamburger-btn" onclick="UI.toggleMobileMenu()" aria-label="Menu">
              <i class="fa-solid fa-bars"></i>
            </button>
          </div>
        </div>
      </div>

      <!-- Mobile Navigation Drawer -->
      <div class="mobile-nav-backdrop" id="mobile-backdrop" onclick="UI.toggleMobileMenu()"></div>
      <div class="mobile-nav-drawer" id="mobile-drawer">
        <div class="mobile-nav-header" style="display:flex; justify-content:space-between; align-items:center; padding:20px; border-bottom:1px solid var(--border-color);">
          <div class="brand-logo">
            <div class="logo-badge"><i class="fa-solid fa-compass"></i></div>
            <div class="brand-name"><span class="brand-name-main">Lang Thang</span></div>
          </div>
          <button class="modal-close-btn" onclick="UI.toggleMobileMenu()">&times;</button>
        </div>
        <ul class="mobile-nav-links" style="list-style:none; padding:16px 0;">
          <li><a href="${rootPath}index.html" class="nav-link ${activePage === 'home' ? 'active' : ''}" style="padding:12px 20px; display:flex; gap:12px;"><i class="fa-solid fa-house"></i> Trang chủ</a></li>
          <li><a href="${pagesPath}search.html" class="nav-link ${activePage === 'search' ? 'active' : ''}" style="padding:12px 20px; display:flex; gap:12px;"><i class="fa-solid fa-compass"></i> Khám phá địa điểm</a></li>
          <li><a href="${pagesPath}foods.html" class="nav-link ${activePage === 'foods' ? 'active' : ''}" style="padding:12px 20px; display:flex; gap:12px;"><i class="fa-solid fa-utensils"></i> Ẩm thực & Quán ngon</a></li>
          <li><a href="${pagesPath}ranking.html" class="nav-link ${activePage === 'ranking' ? 'active' : ''}" style="padding:12px 20px; display:flex; gap:12px;"><i class="fa-solid fa-trophy"></i> Bảng xếp hạng Travelers' Choice</a></li>
          <li><a href="${pagesPath}map.html" class="nav-link ${activePage === 'map' ? 'active' : ''}" style="padding:12px 20px; display:flex; gap:12px;"><i class="fa-solid fa-map-location-dot"></i> Bản đồ tương tác</a></li>
          <li><a href="${pagesPath}notifications.html" class="nav-link ${activePage === 'notifications' ? 'active' : ''}" style="padding:12px 20px; display:flex; gap:12px;"><i class="fa-regular fa-bell"></i> Thông báo của bạn</a></li>
          <li><a href="${pagesPath}favorites.html" class="nav-link" style="padding:12px 20px; display:flex; gap:12px;"><i class="fa-solid fa-heart"></i> Địa điểm yêu thích</a></li>
          <li><a href="${pagesPath}visited.html" class="nav-link" style="padding:12px 20px; display:flex; gap:12px;"><i class="fa-solid fa-calendar-check"></i> Nhật ký chuyến đi</a></li>
          <li><a href="${pagesPath}propose-place.html" class="nav-link" style="padding:12px 20px; display:flex; gap:12px;"><i class="fa-solid fa-plus-circle"></i> Đăng đề xuất địa điểm</a></li>
        </ul>
      </div>
    `;
  }

  // --------------------------------------------------------------------------
  // 4. FOOTER COMPONENT
  // --------------------------------------------------------------------------
  function renderFooter() {
    return `
      <div class="container">
        <div class="footer-grid">
          <div class="footer-brand">
            <a href="${rootPath}index.html" class="brand-logo">
              <div class="logo-badge"><i class="fa-solid fa-compass"></i></div>
              <div class="brand-name">
                <span class="brand-name-main">Lang Thang</span>
              </div>
            </a>
          </div>

          <div class="footer-col">
            <h4>Khám phá</h4>
            <ul class="footer-links">
              <li><a href="${pagesPath}search.html?type=1">Ẩm thực</a></li>
              <li><a href="${pagesPath}search.html?type=2">Điểm tham quan</a></li>
              <li><a href="${pagesPath}search.html?type=3">Khách sạn & Lưu trú</a></li>
              <li><a href="${pagesPath}search.html?type=4">Vui chơi & Giải trí</a></li>
            </ul>
          </div>

          <div class="footer-col">
            <h4>Tính năng</h4>
            <ul class="footer-links">
              <li><a href="${pagesPath}ranking.html">Bảng xếp hạng</a></li>
              <li><a href="${pagesPath}foods.html">Ẩm thực đặc sản</a></li>
              <li><a href="${pagesPath}map.html">Bản đồ du lịch</a></li>
              <li><a href="${pagesPath}propose-place.html">Đóng góp địa điểm</a></li>
            </ul>
          </div>

          <div class="footer-col">
            <h4>Tài khoản</h4>
            <ul class="footer-links">
              <li><a href="${pagesPath}notifications.html">Thông báo</a></li>
              <li><a href="${pagesPath}visited.html">Nhật ký chuyến đi</a></li>
              <li><a href="${pagesPath}favorites.html">Địa điểm đã lưu</a></li>
              <li><a href="${pagesPath}settings.html">Cài đặt tài khoản</a></li>
            </ul>
          </div>
        </div>

        <div class="footer-bottom">
          <div>&copy; 2026 Lang Thang. Bản quyền thuộc về cộng đồng du lịch.</div>
        </div>
      </div>
    `;
  }

  // --------------------------------------------------------------------------
  // 5. PLACE CARD (Grid View)
  // --------------------------------------------------------------------------
  function createPlaceCard(place) {
    const isFav = place.isFavorite || (typeof TravelData !== 'undefined' ? TravelData.isFavorite(place.id) : false);
    
    // Thumbnail fallback
    const thumb = place.thumbnailUrl || place.thumbnail ||
      (place.images && place.images.length > 0 ? place.images[0] : null) ||
      (place.mediaList && place.mediaList.length > 0 ? place.mediaList[0].url : null) ||
      'https://images.unsplash.com/photo-1507525428034-b723cf961d3e?w=600';

    const priceText = place.minPrice
      ? (place.minPrice < 100000 ? '$ · Tiết kiệm' : place.minPrice < 300000 ? '$$ · Vừa phải' : '$$$ · Cao cấp')
      : 'Miễn phí';

    const isSubPageLocal = window.location.pathname.includes('/pages/');
    const detailUrl = `${isSubPageLocal ? './' : 'pages/'}place-detail.html?id=${place.id}`;

    const categoryName = place.category?.name || place.categoryName || place.category || 'Địa điểm';
    const provinceName = place.province?.name || place.provinceName || 'Việt Nam';
    const ratingScore = place.avgRating ? Number(place.avgRating) : 4.8;
    const reviewCount = place.reviewCount || (place.reviews ? place.reviews.length : 120);

    const isTravelersChoice = ratingScore >= 4.5 && reviewCount >= 50;

    const sampleReviewQuote = place.description
      ? place.description.substring(0, 75) + '...'
      : 'Trải nghiệm không gian tuyệt vời, dịch vụ thân thiện và ẩm thực đặc sắc.';

    return `
      <div class="place-card">
        <div class="place-card-img-wrap">
          <a href="${detailUrl}">
            <img src="${thumb}" alt="${place.name}" class="place-card-img" loading="lazy" onerror="this.src='https://images.unsplash.com/photo-1507525428034-b723cf961d3e?w=600'" />
          </a>
          <div class="place-card-badge-top-left">
            ${isTravelersChoice ? `<span class="badge badge-travelers-choice"><i class="fa-solid fa-award"></i> Travelers' Choice</span>` : `<span class="badge badge-category">${categoryName.split('&')[0].trim()}</span>`}
          </div>
          <button class="place-card-favorite-btn ${isFav ? 'active' : ''}"
            onclick="UI.handleFavoriteToggle(${place.id}, this)" title="Lưu vào danh sách yêu thích">
            <i class="fa-${isFav ? 'solid' : 'regular'} fa-heart"></i>
          </button>
        </div>
        <div class="place-card-body">
          <a href="${detailUrl}" class="place-card-title" title="${place.name}">${place.name}</a>
          
          <div style="margin-bottom: 8px;">
            ${renderBubbleRating(ratingScore, reviewCount, true, 'sm')}
          </div>

          <div class="place-card-meta">
            <span>${provinceName}</span>
            <span>·</span>
            <span>${priceText}</span>
          </div>

          <p class="place-card-review-snippet">
            ${sampleReviewQuote}
          </p>
        </div>
      </div>
    `;
  }

  // --------------------------------------------------------------------------
  // 6. TRIPADVISOR LIST CARD (Full-Width Search & Ranking View)
  // --------------------------------------------------------------------------
  function createTripAdvisorListCard(place, rankNumber = null) {
    const isFav = place.isFavorite || (typeof TravelData !== 'undefined' ? TravelData.isFavorite(place.id) : false);
    const thumb = place.thumbnailUrl || place.thumbnail ||
      (place.images && place.images.length > 0 ? place.images[0] : null) ||
      'https://images.unsplash.com/photo-1507525428034-b723cf961d3e?w=600';

    const isSubPageLocal = window.location.pathname.includes('/pages/');
    const detailUrl = `${isSubPageLocal ? './' : 'pages/'}place-detail.html?id=${place.id}`;

    const categoryName = place.category?.name || place.categoryName || place.category || 'Địa điểm';
    const provinceName = place.province?.name || place.provinceName || 'Việt Nam';
    const ratingScore = place.avgRating ? Number(place.avgRating) : 4.8;
    const reviewCount = place.reviewCount || (place.reviews ? place.reviews.length : 120);
    const isTravelersChoice = ratingScore >= 4.5 && reviewCount >= 50;

    return `
      <div class="trip-list-card">
        <div class="trip-list-card-media">
          <a href="${detailUrl}">
            <img src="${thumb}" alt="${place.name}" class="trip-list-card-img" loading="lazy" />
          </a>
          ${rankNumber !== null ? `<div class="trip-list-rank-badge">#${rankNumber}</div>` : ''}
          <button class="place-card-favorite-btn ${isFav ? 'active' : ''}"
            onclick="UI.handleFavoriteToggle(${place.id}, this)" title="Lưu yêu thích" style="top:12px; right:12px;">
            <i class="fa-${isFav ? 'solid' : 'regular'} fa-heart"></i>
          </button>
        </div>

        <div class="trip-list-card-content">
          <div>
            <div style="display:flex; justify-content:space-between; align-items:flex-start; gap:12px; margin-bottom:4px;">
              <a href="${detailUrl}" class="trip-list-card-title">
                ${rankNumber !== null ? `${rankNumber}. ` : ''}${place.name}
              </a>
              ${isTravelersChoice ? renderTravelersChoiceBadge() : ''}
            </div>

            <div style="margin-bottom: 8px;">
              ${renderBubbleRating(ratingScore, reviewCount, true)}
            </div>

            <div class="trip-list-card-tags">
              <span class="trip-tag trip-tag-category">${categoryName}</span>
              <span class="trip-tag">${provinceName}</span>
              ${place.openingHours ? `<span class="trip-tag">${place.openingHours}</span>` : ''}
            </div>
          </div>

          <p class="trip-list-card-desc">${place.description || 'Địa điểm du lịch và trải nghiệm tuyệt vời với đầy đủ dịch vụ tiện ích hàng đầu.'}</p>

          <div class="trip-list-card-footer">
            <div class="trip-price-highlight">
              ${place.minPrice ? `${Number(place.minPrice).toLocaleString('vi-VN')}đ — ${Number(place.maxPrice || place.minPrice * 2).toLocaleString('vi-VN')}đ` : 'Miễn phí vé vào cổng'}
            </div>
            <a href="${detailUrl}" class="btn btn-primary btn-sm" style="padding:7px 18px; font-size:0.85rem; font-weight:700;">
              Xem chi tiết <i class="fa-solid fa-arrow-right" style="margin-left:4px; font-size:0.75rem;"></i>
            </a>
          </div>
        </div>
      </div>
    `;
  }

  // --------------------------------------------------------------------------
  // 7. TOAST NOTIFICATIONS
  // --------------------------------------------------------------------------
  function showToast(message, type = 'info') {
    let container = document.getElementById('toast-container');
    if (!container) {
      container = document.createElement('div');
      container.id = 'toast-container';
      container.style.position = 'fixed';
      container.style.bottom = '24px';
      container.style.right = '24px';
      container.style.zIndex = '999999';
      container.style.display = 'flex';
      container.style.flexDirection = 'column';
      container.style.gap = '8px';
      document.body.appendChild(container);
    }

    const toast = document.createElement('div');
    toast.className = `toast toast-${type}`;
    
    let borderColor = 'var(--accent)';
    let bgColor = '#ffffff';

    if (type === 'success') {
      borderColor = 'var(--emerald)';
    } else if (type === 'error' || type === 'danger') {
      borderColor = 'var(--rose)';
    } else if (type === 'warning') {
      borderColor = 'var(--amber-dark)';
    }

    toast.style.cssText = `
      min-width: 280px;
      max-width: 420px;
      background: ${bgColor};
      border-left: 4px solid ${borderColor};
      padding: 14px 18px;
      border-radius: var(--radius-md);
      box-shadow: var(--shadow-lg);
      display: flex;
      align-items: center;
      gap: 12px;
      font-size: 0.9rem;
      font-weight: 600;
      color: var(--text-main);
      animation: slideInRight 0.3s ease;
      transition: all 0.25s ease;
    `;

    toast.innerHTML = `
      <span style="flex:1;">${message}</span>
      <button style="background:none; border:none; color:var(--text-muted); cursor:pointer; font-size:1.1rem; padding:0 4px;" onclick="this.parentElement.remove()">&times;</button>
    `;

    container.appendChild(toast);

    setTimeout(() => {
      toast.style.opacity = '0';
      toast.style.transform = 'translateX(100%)';
      setTimeout(() => toast.remove(), 250);
    }, 3500);
  }

  // --------------------------------------------------------------------------
  // 8. FAVORITE TOGGLE (API or LocalStorage)
  // --------------------------------------------------------------------------
  async function handleFavoriteToggle(placeId, btn) {
    if (typeof api !== 'undefined') {
      const user = api.getCurrentUser();
      if (!user) {
        showToast('Vui lòng đăng nhập để lưu yêu thích', 'warning');
        return;
      }
      try {
        const res = await fetch(`/api/User/favorites/${placeId}?userId=${user.id}`, { method: 'POST' });
        const data = await res.json();
        const isNowFav = data.isFavorite;
        btn.classList.toggle('active', isNowFav);
        const icon = btn.querySelector('i');
        if (icon) icon.className = `fa-${isNowFav ? 'solid' : 'regular'} fa-heart`;
        showToast(data.message, isNowFav ? 'success' : 'info');
        return;
      } catch (e) {
        // Fallback to local storage
      }
    }

    if (typeof TravelData !== 'undefined') {
      const isNowFav = TravelData.toggleFavorite(placeId);
      btn.classList.toggle('active', isNowFav);
      const icon = btn.querySelector('i');
      if (icon) icon.className = `fa-${isNowFav ? 'solid' : 'regular'} fa-heart`;
      showToast(isNowFav ? 'Đã thêm vào danh sách yêu thích!' : 'Đã xóa khỏi yêu thích!', isNowFav ? 'success' : 'info');
    }
  }

  // --------------------------------------------------------------------------
  // 9. USER DROPDOWN & LOGOUT
  // --------------------------------------------------------------------------
  function toggleUserDropdown(e) {
    e.stopPropagation();
    const menu = document.getElementById('headerDropdownMenu');
    const notiMenu = document.getElementById('headerNotificationDropdown');
    if (notiMenu) notiMenu.style.display = 'none';

    if (menu) menu.classList.toggle('active');
    document.addEventListener('click', () => {
      if (menu) menu.classList.remove('active');
    }, { once: true });
  }

  function handleLogout(e) {
    if (e) {
      e.preventDefault();
      e.stopPropagation();
    }
    localStorage.clear();
    sessionStorage.clear();
    if (typeof api !== 'undefined') {
      try { api.setCurrentUser(null); } catch (err) {}
    }
    if (typeof TravelData !== 'undefined') {
      try { TravelData.logoutUser(); } catch (err) {}
    }
    const isSub = window.location.pathname.includes('/pages/');
    window.location.href = (isSub ? '../index.html' : 'index.html') + '?t=' + Date.now();
  }

  // Also expose globally on window
  window.handleLogout = handleLogout;
  window.logout = handleLogout;

  function toggleMobileMenu() {
    const drawer = document.getElementById('mobile-drawer');
    const backdrop = document.getElementById('mobile-backdrop');
    if (drawer && backdrop) {
      drawer.classList.toggle('active');
      backdrop.classList.toggle('active');
    }
  }

  function openModal(modalId) {
    const m = document.getElementById(modalId);
    if (m) m.classList.add('active');
  }

  function closeModal(modalId) {
    const m = document.getElementById(modalId);
    if (m) m.classList.remove('active');
  }

  // --------------------------------------------------------------------------
  // 10. NOTIFICATION POPOVER (IN-PLACE DROPDOWN)
  // --------------------------------------------------------------------------
  function toggleNotificationDropdown(e) {
    e.stopPropagation();
    const dropdown = document.getElementById('headerNotificationDropdown');
    if (!dropdown) return;
    
    const isShowing = dropdown.style.display === 'block';
    
    // Hide other dropdowns
    const userMenu = document.getElementById('headerDropdownMenu');
    if (userMenu) userMenu.classList.remove('active');
    
    if (!isShowing) {
      dropdown.style.display = 'block';
      renderDropdownNotifications();
      
      const closeDropdown = (evt) => {
        if (!dropdown.contains(evt.target)) {
          dropdown.style.display = 'none';
          document.removeEventListener('click', closeDropdown);
        }
      };
      setTimeout(() => {
        document.addEventListener('click', closeDropdown);
      }, 10);
    } else {
      dropdown.style.display = 'none';
    }
  }

  function renderDropdownNotifications() {
    const listContainer = document.getElementById('headerNotificationList');
    if (!listContainer || typeof TravelData === 'undefined') return;

    const notis = TravelData.getNotifications();
    if (notis.length === 0) {
      listContainer.innerHTML = '<div class="noti-panel-empty"><span>Không có thông báo nào</span></div>';
      return;
    }

    listContainer.innerHTML = notis.map(n => `
      <div class="noti-item ${n.isRead ? '' : 'unread'}" onclick="UI.markNotiReadFromDropdown(${n.id}, event)" role="button">
        <div class="noti-item-body">
          <div style="display:flex; justify-content:space-between; align-items:center; gap:8px; margin-bottom:2px;">
            <span class="noti-item-title">${n.title}</span>
            <span class="noti-item-time">${n.time}</span>
          </div>
          <div class="noti-item-msg">${n.content}</div>
        </div>
        ${!n.isRead ? '<div class="noti-item-dot"></div>' : ''}
      </div>
    `).join('');
  }

  function markNotiReadFromDropdown(id, e) {
    if (e) e.stopPropagation();
    if (typeof TravelData !== 'undefined') {
      const notis = TravelData.getNotifications();
      const noti = notis.find(n => n.id === parseInt(id));
      TravelData.markNotificationRead(id);
      renderDropdownNotifications();
      updateNotificationBadge();

      if (noti && noti.targetUrl) {
        const isSubPageLocal = window.location.pathname.includes('/pages/');
        const target = noti.targetUrl.startsWith('http') || noti.targetUrl.startsWith('/')
          ? noti.targetUrl
          : ((isSubPageLocal ? './' : 'pages/') + noti.targetUrl);
        window.location.href = target;
      }
    }
  }

  function markAllNotiRead(e) {
    if (e) e.stopPropagation();
    if (typeof TravelData !== 'undefined') {
      TravelData.markAllNotificationsRead();
      renderDropdownNotifications();
      updateNotificationBadge();
      showToast('Đã đánh dấu tất cả thông báo là đã đọc!', 'success');
    }
  }

  function updateNotificationBadge() {
    if (typeof TravelData !== 'undefined') {
      const notis = TravelData.getNotifications();
      const hasUnread = notis.some(n => !n.isRead);
      const dot = document.getElementById('header-unread-dot');
      if (dot) dot.style.display = hasUnread ? 'block' : 'none';
    }
  }

  return {
    renderBubbleRating,
    renderTravelersChoiceBadge,
    renderHeader,
    renderFooter,
    createPlaceCard,
    createTripAdvisorListCard,
    showToast,
    handleFavoriteToggle,
    toggleMobileMenu,
    toggleUserDropdown,
    handleLogout,
    openModal,
    closeModal,
    toggleNotificationDropdown,
    renderDropdownNotifications,
    markNotiReadFromDropdown,
    markAllNotiRead,
    updateNotificationBadge
  };
})();

// Auto update badge on DOM ready
if (typeof document !== 'undefined') {
  document.addEventListener('DOMContentLoaded', () => {
    setTimeout(() => {
      if (typeof UI !== 'undefined' && UI.updateNotificationBadge) {
        UI.updateNotificationBadge();
      }
    }, 100);
  });
}
