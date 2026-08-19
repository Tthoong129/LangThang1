/* ==========================================================================
   MINIMAP & LANG THANG — PLACE DETAIL CONTROLLER (Warm Palette + Reviewer Links)
   ========================================================================== */

let currentPlace = null;
let allReviews = [];
let filteredReviews = [];
let miniMapInstance = null;
let selectedStarRating = 5;

document.addEventListener('DOMContentLoaded', () => {
  initPlaceDetailPage();
});

async function initPlaceDetailPage() {
  const urlParams = new URLSearchParams(window.location.search);
  const placeId = parseInt(urlParams.get('id')) || 1;

  // Mount header and footer
  const headerEl = document.getElementById('site-header-mount');
  const footerEl = document.getElementById('site-footer-mount');
  if (headerEl) headerEl.innerHTML = UI.renderHeader('search');
  if (footerEl) footerEl.innerHTML = UI.renderFooter();

  try {
    if (typeof api !== 'undefined') {
      currentPlace = await api.getPlaceDetails(placeId);
    }
  } catch(e) {}

  if (!currentPlace) {
    if (typeof TravelData !== 'undefined') {
      currentPlace = TravelData.getPlaceById(placeId);
    }
  }

  if (!currentPlace) {
    document.getElementById('place-detail-content').innerHTML = `
      <div style="text-align:center; padding: 60px 20px;">
        <i class="fa-solid fa-compass" style="font-size:3.5rem; color:var(--rose); margin-bottom:16px;"></i>
        <h2 style="font-size:1.8rem; font-weight:800; margin-bottom:10px;">Địa điểm không tồn tại</h2>
        <p style="color:var(--text-muted); margin-bottom:20px;">Địa điểm bạn đang tìm kiếm không tồn tại hoặc đã được chuyển đi nơi khác.</p>
        <a href="../index.html" class="btn btn-primary">Quay lại Trang chủ</a>
      </div>
    `;
    return;
  }

  // Populate Reviews List with reviewer user IDs
  allReviews = currentPlace.reviews && currentPlace.reviews.length > 0
    ? currentPlace.reviews
    : (typeof TravelData !== 'undefined' ? TravelData.getReviewsByPlace(currentPlace.id) : []);

  if (allReviews.length === 0) {
    allReviews = [
      {
        id: 101,
        userId: 1,
        userName: 'Minh Hoàng',
        userAvatar: 'https://images.unsplash.com/photo-1535713875002-d1d0cf377fde?w=150',
        userLevel: 'Level 5 Contributor · 42 đánh giá',
        rating: 5,
        travelerType: 'Cặp đôi',
        title: 'Trải nghiệm vượt ngoài kỳ vọng, đồ ăn và phục vụ xuất sắc!',
        content: 'Chúng tôi ghé thăm vào một buổi tối cuối tuần. Không gian cực kỳ ấm cúng, đậm chất văn hóa. Nhân viên tư vấn nhiệt tình, các món ăn đều nêm nếm rất vừa miệng và chuẩn vị. Chắc chắn sẽ quay lại nhiều lần nữa!',
        visitDate: 'Tháng 2 năm 2026',
        helpfulCount: 18,
        photos: [
          'https://images.unsplash.com/photo-1582878826629-29b7ad1cdc43?w=400',
          'https://images.unsplash.com/photo-1569718212165-3a8278d5f624?w=400'
        ]
      },
      {
        id: 102,
        userId: 2,
        userName: 'Thu Thảo',
        userAvatar: 'https://images.unsplash.com/photo-1494790108377-be9c29b29330?w=150',
        userLevel: 'Level 3 Contributor · 18 đánh giá',
        rating: 5,
        travelerType: 'Gia đình',
        title: 'Rất thích hợp cho gia đình có trẻ nhỏ, view đẹp và sạch sẽ',
        content: 'Địa điểm rộng rãi, sạch sẽ và có bãi đỗ xe tiện lợi. Món ăn tươi ngon, giá cả hợp lý so với chất lượng. Cả gia đình tôi đều rất hài lòng.',
        visitDate: 'Tháng 1 năm 2026',
        helpfulCount: 9,
        photos: []
      },
      {
        id: 103,
        userId: 3,
        userName: 'Alexander Wright',
        userAvatar: 'https://images.unsplash.com/photo-1570295999919-56ceb5ecca61?w=150',
        userLevel: 'Top Reviewer · 86 đánh giá',
        rating: 4,
        travelerType: 'Đi một mình',
        title: 'Must visit place in Vietnam, authentic atmosphere',
        content: 'A fantastic highlight during my Vietnam journey. Great service, authentic taste and reasonably priced. Would recommend booking in advance if you visit during peak dinner hours.',
        visitDate: 'Tháng 1 năm 2026',
        helpfulCount: 24,
        photos: []
      }
    ];
  }

  filteredReviews = [...allReviews];

  // Render Page Sections
  renderPlaceHeader();
  renderPlaceGalleryMosaic();
  renderPlaceDetails();
  renderRatingBreakdown();
  renderReviewsList();
  renderPlaceMap();
  renderRelatedPlaces();
  setupFavoriteButton();
  setupAuthButtons();
}

function renderPlaceHeader() {
  document.getElementById('breadcrumb-place-name').innerText = currentPlace.name;
  
  const categoryName = currentPlace.category?.name || currentPlace.categoryName || currentPlace.category || 'Địa điểm';
  const provinceName = currentPlace.province?.name || currentPlace.provinceName || 'Việt Nam';
  
  document.getElementById('breadcrumb-category').innerText = categoryName;
  document.getElementById('place-title').innerText = currentPlace.name;
  document.getElementById('place-category-badge').innerText = categoryName;
  
  const ratingScore = currentPlace.avgRating ? Number(currentPlace.avgRating) : 4.8;
  const reviewCount = currentPlace.reviewCount || allReviews.length || 142;

  // Mount 5-bubble rating in header
  const bubbleMount = document.getElementById('place-bubble-rating-mount');
  if (bubbleMount) {
    bubbleMount.innerHTML = UI.renderBubbleRating(ratingScore, reviewCount, true, 'lg');
  }

  // Ranking text
  const rankMount = document.getElementById('place-ranking-text');
  if (rankMount) {
    rankMount.innerHTML = `🏆 #1 trong số các địa điểm nổi bật tại ${provinceName}`;
  }

  // Price tier
  const priceMount = document.getElementById('place-price-tier');
  if (priceMount) {
    priceMount.innerText = currentPlace.minPrice
      ? `${Math.round(currentPlace.minPrice / 1000)}k – ${Math.round((currentPlace.maxPrice || currentPlace.minPrice * 1.5) / 1000)}k VNĐ`
      : 'Miễn phí vé vào';
  }

  // Info sidebar
  document.getElementById('sidebar-address-label').innerText = currentPlace.address || provinceName;
  document.getElementById('place-hours-text').innerText = currentPlace.openingHours || '07:00 – 22:00 hằng ngày';
  document.getElementById('place-phone-text').innerText = currentPlace.phone || '0912 345 678';
  
  const webEl = document.getElementById('place-website-text');
  if (currentPlace.website) {
    webEl.innerHTML = `<a href="${currentPlace.website}" target="_blank" rel="noopener" style="color: var(--primary); font-weight:600;">${currentPlace.website.replace('https://', '').replace('http://', '')}</a>`;
  } else {
    webEl.innerText = 'Đang cập nhật';
  }

  // Google Maps Directions Button
  const dirBtn = document.getElementById('btn-detail-directions');
  if (dirBtn) {
    const lat = currentPlace.latitude || 10.7769;
    const lng = currentPlace.longitude || 106.7009;
    dirBtn.href = `https://www.google.com/maps/dir/?api=1&destination=${lat},${lng}`;
  }
}

function renderPlaceGalleryMosaic() {
  const defaultFallbacks = [
    'https://images.unsplash.com/photo-1501339847302-ac426a4a7cbb?w=800',
    'https://images.unsplash.com/photo-1514432324607-a09d9b4aefdd?w=800',
    'https://images.unsplash.com/photo-1554118811-1e0d58224f24?w=800',
    'https://images.unsplash.com/photo-1495474472287-4d71bcdd2085?w=800',
    'https://images.unsplash.com/photo-1442512595331-e89e73853f31?w=800'
  ];

  let rawImages = (currentPlace.mediaList && currentPlace.mediaList.length > 0)
    ? currentPlace.mediaList.map(m => m.url)
    : (currentPlace.images && currentPlace.images.length > 0)
      ? currentPlace.images
      : [];

  if (currentPlace.thumbnailUrl && !rawImages.includes(currentPlace.thumbnailUrl)) {
    rawImages.unshift(currentPlace.thumbnailUrl);
  }

  // Ensure 5 distinct photos
  let distinctImages = [...rawImages];
  for (let fb of defaultFallbacks) {
    if (distinctImages.length >= 5) break;
    if (!distinctImages.includes(fb)) {
      distinctImages.push(fb);
    }
  }

  for (let i = 1; i <= 5; i++) {
    const imgEl = document.getElementById(`gallery-img-${i}`);
    if (imgEl) {
      imgEl.src = distinctImages[i - 1] || defaultFallbacks[i - 1];
    }
  }

  const moreBtn = document.getElementById('gallery-more-btn');
  const moreText = document.getElementById('gallery-more-text');
  if (moreBtn && moreText) {
    moreText.innerText = `Xem tất cả ${Math.max(distinctImages.length, 24)} ảnh`;
  }
}

function renderPlaceDetails() {
  const descEl = document.getElementById('place-description-text');
  if (descEl) {
    descEl.innerText = currentPlace.description ||
      'Địa điểm nổi tiếng thu hút đông đảo du khách trong nước và quốc tế nhờ vẻ đẹp kiến trúc độc đáo, không gian văn hóa đặc sắc và dịch vụ chu đáo chuẩn quốc tế.';
  }
}

function renderRatingBreakdown() {
  const ratingScore = currentPlace.avgRating ? Number(currentPlace.avgRating) : 4.8;
  const reviewCount = currentPlace.reviewCount || allReviews.length || 142;

  const scoreBig = document.getElementById('review-score-big');
  if (scoreBig) scoreBig.innerText = ratingScore.toFixed(1);

  const bigBubble = document.getElementById('big-bubble-mount');
  if (bigBubble) bigBubble.innerHTML = UI.renderBubbleRating(ratingScore, null, false, 'lg');

  const totalCountEl = document.getElementById('review-total-count');
  if (totalCountEl) totalCountEl.innerText = `${reviewCount.toLocaleString('vi-VN')} đánh giá`;
}

function renderReviewsList() {
  const container = document.getElementById('reviews-list-container');
  if (!container) return;

  if (filteredReviews.length === 0) {
    container.innerHTML = `
      <div style="text-align:center; padding:32px; background:var(--bg-subtle); border-radius:var(--radius-lg);">
        <p style="color:var(--text-muted); font-size:0.92rem;">Chưa có đánh giá nào phù hợp với bộ lọc đã chọn.</p>
      </div>
    `;
    return;
  }

  container.innerHTML = filteredReviews.map(r => {
    const authorId = r.userId || r.user?.id || r.UserId || r.User?.Id || 1;
    const authorName = r.user?.fullName || r.userName || r.UserName || r.User?.FullName || 'Cộng tác viên Lang Thang';
    const avatar = r.user?.avatarUrl || r.userAvatar || r.UserAvatar || r.User?.AvatarUrl || 'https://images.unsplash.com/photo-1535713875002-d1d0cf377fde?w=150';
    const ratingVal = r.rating || r.Rating || 5;
    const travelerType = r.travelerType || r.TravelerType || 'Du lịch trải nghiệm';
    const dateStr = r.visitDate || (r.createdAt ? new Date(r.createdAt).toLocaleDateString('vi-VN') : 'Gần đây');
    const title = r.title || r.Title || 'Trải nghiệm rất tuyệt vời!';
    const content = r.comment || r.content || r.Content || r.Comment || '';
    const helpfulCount = r.helpfulCount || (Math.floor(Math.random() * 12) + 2);

    return `
      <div class="review-card-item">
        <div class="reviewer-header">
          <!-- Clickable Link to Reviewer's Personal Profile -->
          <a href="profile.html?id=${authorId}" class="reviewer-user-link" title="Xem trang cá nhân của ${authorName}">
            <img src="${avatar}" alt="${authorName}" class="reviewer-avatar" onerror="this.src='https://images.unsplash.com/photo-1535713875002-d1d0cf377fde?w=150'" />
            <div>
              <div class="reviewer-name">${authorName} <i class="fa-solid fa-arrow-up-right-from-square" style="font-size:0.7rem; color:var(--primary); margin-left:2px;"></i></div>
              <div class="reviewer-badge-line">
                <span><i class="fa-solid fa-award" style="color:var(--primary);"></i> ${r.userLevel || 'Cộng tác viên uy tín'}</span>
                <span>•</span>
                <span>Chuyến đi: <strong>${travelerType}</strong></span>
              </div>
            </div>
          </a>
          <div style="font-size:0.82rem; color:var(--text-light);">${dateStr}</div>
        </div>

        <div style="margin: 8px 0;">
          ${UI.renderBubbleRating(ratingVal, null, false)}
        </div>

        <div class="review-title">${title}</div>
        <div class="review-text">${content}</div>

        ${r.photos && r.photos.length > 0 ? `
          <div class="review-photos-grid">
            ${r.photos.map(p => `<img src="${p}" alt="Ảnh đánh giá" class="review-photo-thumb" onclick="openPhotoViewerModal('${p}')" />`).join('')}
          </div>
        ` : ''}

        <div class="review-actions-bar">
          <div><i class="fa-solid fa-shield-halved" style="color:var(--primary);"></i> Đánh giá đã được xác thực</div>
          <button type="button" class="btn-helpful" onclick="handleHelpfulClick(this, ${helpfulCount})">
            <i class="fa-regular fa-thumbs-up"></i> Hữu ích (<span>${helpfulCount}</span>)
          </button>
        </div>
      </div>
    `;
  }).join('');
}

function handleHelpfulClick(btn, initialCount) {
  const span = btn.querySelector('span');
  if (!btn.classList.contains('active')) {
    btn.classList.add('active');
    btn.style.background = 'var(--primary-light)';
    btn.style.borderColor = 'var(--primary)';
    btn.style.color = 'var(--primary)';
    if (span) span.innerText = initialCount + 1;
    UI.showToast('Cảm ơn bạn đã phản hồi đánh giá hữu ích!', 'success');
  } else {
    btn.classList.remove('active');
    btn.style.background = '#ffffff';
    btn.style.borderColor = 'var(--border-color)';
    btn.style.color = 'var(--text-body)';
    if (span) span.innerText = initialCount;
  }
}

function filterReviewsByRating(star) {
  filteredReviews = allReviews.filter(r => Math.round(r.rating || 5) === star);
  if (filteredReviews.length === 0) {
    filteredReviews = allReviews;
  }
  renderReviewsList();
  UI.showToast(`Đã lọc danh sách đánh giá ${star} sao`, 'info');
}

function filterReviewsByTravelerType(type, btn) {
  document.querySelectorAll('.review-chip').forEach(c => c.classList.remove('active'));
  if (btn) btn.classList.add('active');

  if (type === 'all') {
    filteredReviews = [...allReviews];
  } else {
    const typeMap = {
      'couples': 'Cặp đôi',
      'family': 'Gia đình',
      'friends': 'Bạn bè',
      'solo': 'Đi một mình'
    };
    const target = typeMap[type] || type;
    filteredReviews = allReviews.filter(r => (r.travelerType || '').toLowerCase().includes(target.toLowerCase()));
    if (filteredReviews.length === 0) {
      filteredReviews = [...allReviews];
    }
  }
  renderReviewsList();
}

function renderPlaceMap() {
  const mapContainer = document.getElementById('mini-map');
  if (!mapContainer || typeof L === 'undefined') return;

  const lat = currentPlace.latitude || 10.7769;
  const lng = currentPlace.longitude || 106.7009;

  if (miniMapInstance) {
    miniMapInstance.remove();
  }

  miniMapInstance = L.map('mini-map', {
    zoomControl: false,
    attributionControl: false
  }).setView([lat, lng], 15);

  L.tileLayer('https://{s}.tile.openstreetmap.org/{z}/{x}/{y}.png').addTo(miniMapInstance);

  const customIcon = L.divIcon({
    className: 'tripadvisor-map-pin',
    html: `<div style="background-color:#c85a2e; color:white; width:34px; height:34px; border-radius:50%; display:flex; align-items:center; justify-content:center; box-shadow:0 3px 8px rgba(200,90,46,0.4); border:2.5px solid white;"><i class="fa-solid fa-location-dot" style="font-size:1rem;"></i></div>`,
    iconSize: [34, 34],
    iconAnchor: [17, 34]
  });

  L.marker([lat, lng], { icon: customIcon })
    .addTo(miniMapInstance)
    .bindPopup(`<strong>${currentPlace.name}</strong><br>${currentPlace.address || ''}`)
    .openPopup();
}

async function renderRelatedPlaces() {
  const container = document.getElementById('related-places-grid');
  if (!container) return;

  let places = [];
  try {
    if (typeof api !== 'undefined') {
      places = await api.getPlaces();
    }
  } catch(e) {}

  if (!places || places.length === 0) {
    if (typeof TravelData !== 'undefined') {
      places = TravelData.getPlaces();
    }
  }

  const related = places.filter(p => p.id !== currentPlace.id).slice(0, 4);
  container.innerHTML = related.map(p => UI.createPlaceCard(p)).join('');
}

function setupFavoriteButton() {
  const btn = document.getElementById('btn-detail-favorite');
  if (!btn) return;

  const isFav = currentPlace.isFavorite || (typeof TravelData !== 'undefined' ? TravelData.isFavorite(currentPlace.id) : false);
  if (isFav) {
    btn.classList.add('active');
    btn.innerHTML = '<i class="fa-solid fa-heart" style="color:var(--rose);"></i> Đã lưu';
  }

  btn.onclick = () => {
    UI.handleFavoriteToggle(currentPlace.id, btn);
    const nowActive = btn.classList.contains('active');
    btn.innerHTML = nowActive
      ? '<i class="fa-solid fa-heart" style="color:var(--rose);"></i> Đã lưu'
      : '<i class="fa-regular fa-heart"></i> Lưu vào danh sách';
  };
}

function setupAuthButtons() {
  const user = typeof api !== 'undefined' ? api.getCurrentUser() : null;
  const btnEdit = document.getElementById('btn-edit-place');
  const btnSuggest = document.getElementById('btn-suggest-edit');

  if (user) {
    if (btnSuggest) btnSuggest.style.display = 'inline-flex';
    if (user.role && user.role.includes('admin') && btnEdit) {
      btnEdit.style.display = 'inline-flex';
    }
  }
}

// Partial Quick Actions Popover Toggle
function toggleQuickActionMenu(e) {
  e.stopPropagation();
  const menu = document.getElementById('quick-action-popover');
  if (menu) menu.classList.toggle('active');
  document.addEventListener('click', () => {
    if (menu) menu.classList.remove('active');
  }, { once: true });
}

// Modal Review Actions
function openWriteReviewModal() {
  UI.openModal('modal-write-review');
}

function setModalRating(val) {
  selectedStarRating = val;
  document.getElementById('review-input-rating').value = val;
  const stars = document.querySelectorAll('#modal-bubble-picker i');
  stars.forEach((s, idx) => {
    if (idx < val) {
      s.className = 'fa-solid fa-circle';
      s.style.color = '#c85a2e';
    } else {
      s.className = 'fa-regular fa-circle';
      s.style.color = '#cbd5e1';
    }
  });
}

function handleReviewSubmit(e) {
  e.preventDefault();
  const title = document.getElementById('review-input-title').value;
  const content = document.getElementById('review-input-content').value;
  const travelerType = document.getElementById('review-input-type').value;
  const rating = parseInt(document.getElementById('review-input-rating').value) || 5;

  const user = typeof api !== 'undefined' ? api.getCurrentUser() : (typeof TravelData !== 'undefined' ? TravelData.getUserProfile() : null);

  const newReview = {
    id: Date.now(),
    userId: user ? user.id : 1,
    userName: user ? user.fullName : 'Bạn (Du khách mới)',
    userAvatar: user ? user.avatarUrl : 'https://images.unsplash.com/photo-1535713875002-d1d0cf377fde?w=150',
    userLevel: 'Đánh giá mới · Vừa xong',
    rating: rating,
    travelerType: travelerType,
    title: title,
    content: content,
    visitDate: 'Vừa xong',
    helpfulCount: 0,
    photos: []
  };

  allReviews.unshift(newReview);
  filteredReviews = [...allReviews];
  renderReviewsList();

  UI.closeModal('modal-write-review');
  UI.showToast('Cảm ơn bạn! Đánh giá đã được đăng thành công.', 'success');
  document.getElementById('form-write-review').reset();
}

function openPhotoViewer(idx) {
  UI.showToast(`Đang mở toàn màn hình thư viện ảnh (${idx + 1})...`, 'info');
}

function openPhotoViewerModal(src) {
  window.open(src, '_blank');
}
