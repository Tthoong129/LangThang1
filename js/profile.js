/* ==========================================================================
   MINIMAP & LANG THANG — USER PROFILE & REVIEWS FEED CONTROLLER
   ========================================================================== */

document.addEventListener('DOMContentLoaded', () => {
  initProfilePage();
});

async function initProfilePage() {
  const headerEl = document.getElementById('site-header-mount');
  const footerEl = document.getElementById('site-footer-mount');
  if (headerEl) headerEl.innerHTML = UI.renderHeader('profile');
  if (footerEl) footerEl.innerHTML = UI.renderFooter();

  const urlParams = new URLSearchParams(window.location.search);
  const userId = parseInt(urlParams.get('id')) || 1;

  await loadProfileData(userId);
  await loadReviewsFeed(userId);
  loadVisitedPlaces(userId);
}

async function loadProfileData(userId) {
  let profile = null;

  try {
    if (typeof api !== 'undefined') {
      const res = await fetch(`/api/User/${userId}/profile`);
      if (res.ok) profile = await res.json();
    }
  } catch (error) {}

  if (!profile) {
    // Mock user dataset
    const mockUsers = {
      1: {
        fullName: 'Minh Hoàng',
        avatarUrl: 'https://images.unsplash.com/photo-1535713875002-d1d0cf377fde?w=150',
        bio: 'Đam mê khám phá các quán ăn đường phố và danh lam thắng cảnh cổ kính khắp 3 miền Việt Nam.',
        reviewCount: 42,
        helpfulCount: 156,
        joinedAt: '2024-03-15T00:00:00Z',
        level: 'Level 5 Contributor'
      },
      2: {
        fullName: 'Thu Thảo',
        avatarUrl: 'https://images.unsplash.com/photo-1494790108377-be9c29b29330?w=150',
        bio: 'Yêu thích du lịch cùng gia đình, thích các quán cà phê view đẹp và không gian yên tĩnh.',
        reviewCount: 18,
        helpfulCount: 64,
        joinedAt: '2024-08-20T00:00:00Z',
        level: 'Level 3 Contributor'
      },
      3: {
        fullName: 'Alexander Wright',
        avatarUrl: 'https://images.unsplash.com/photo-1570295999919-56ceb5ecca61?w=150',
        bio: 'Travel enthusiast exploring Southeast Asia cuisine and heritage sites.',
        reviewCount: 86,
        helpfulCount: 312,
        joinedAt: '2023-11-10T00:00:00Z',
        level: 'Top Reviewer 2024'
      }
    };

    profile = mockUsers[userId] || {
      fullName: 'Du Khách Lang Thang',
      avatarUrl: 'https://images.unsplash.com/photo-1535713875002-d1d0cf377fde?w=150',
      bio: 'Thành viên cộng đồng du lịch Lang Thang.',
      reviewCount: 12,
      helpfulCount: 34,
      joinedAt: '2025-01-01T00:00:00Z',
      level: 'Level 2 Contributor'
    };
  }

  document.getElementById('profileName').textContent = profile.fullName;
  document.getElementById('profileAvatar').src = profile.avatarUrl || 'https://images.unsplash.com/photo-1535713875002-d1d0cf377fde?w=150';
  document.getElementById('profileBio').textContent = profile.bio || 'Thành viên cộng đồng du lịch Lang Thang.';
  document.getElementById('profileReviewCount').textContent = profile.reviewCount || 0;
  
  const helpfulEl = document.getElementById('profileHelpfulCount');
  if (helpfulEl) helpfulEl.textContent = profile.helpfulCount || 48;

  const badgeEl = document.getElementById('profileLevelBadge');
  if (badgeEl) badgeEl.innerHTML = `<i class="fa-solid fa-award"></i> ${profile.level || 'Cộng tác viên uy tín'}`;

  const tabReviewsCount = document.getElementById('tab-reviews-count');
  if (tabReviewsCount) tabReviewsCount.textContent = profile.reviewCount || 0;

  const date = new Date(profile.joinedAt || '2024-01-01');
  document.getElementById('profileJoined').innerHTML = `<i class="fa-regular fa-calendar"></i> Tham gia tháng ${date.getMonth() + 1} năm ${date.getFullYear()}`;
}

async function loadReviewsFeed(userId) {
  const feedContainer = document.getElementById('reviewsFeed');
  if (!feedContainer) return;

  let reviews = [];
  try {
    if (typeof api !== 'undefined') {
      const res = await fetch(`/api/User/${userId}/reviews`);
      if (res.ok) reviews = await res.json();
    }
  } catch (error) {}

  if (!reviews || reviews.length === 0) {
    // Mock user reviews
    reviews = [
      {
        placeId: 1,
        placeName: 'Bún Bò Huế Đông Ba',
        placeCategory: 'Ăn uống',
        placeProvince: 'TP. Hồ Chí Minh',
        placeRating: 4.8,
        placeReviewCount: 320,
        rating: 5,
        title: 'Nước dùng đậm đà, chuẩn vị Cố Đô!',
        content: 'Bún bò ở đây rất chuẩn vị miền Trung, nước dùng thơm mùi sả và ruốc huế nhưng không bị gắt. Thịt bò bắp mềm, chả cua dai ngọt tự nhiên. Phục vụ nhanh và thân thiện.',
        createdAt: '2026-02-14T00:00:00Z',
        travelerType: 'Cặp đôi',
        images: ['https://images.unsplash.com/photo-1582878826629-29b7ad1cdc43?w=500']
      },
      {
        placeId: 2,
        placeName: 'Nhà Thờ Đức Bà Sài Gòn',
        placeCategory: 'Du lịch',
        placeProvince: 'TP. Hồ Chí Minh',
        placeRating: 4.7,
        placeReviewCount: 1250,
        rating: 5,
        title: 'Biểu tượng kiến trúc tuyệt đẹp ngay trung tâm thành phố',
        content: 'Một công trình kiến trúc Gothic tuyệt mỹ. Buổi sáng ngồi cà phê bệt ngắm nhà thờ và chim bồ câu là một trải nghiệm đậm chất Sài Gòn không thể bỏ qua.',
        createdAt: '2026-01-20T00:00:00Z',
        travelerType: 'Bạn bè',
        images: []
      }
    ];
  }

  feedContainer.innerHTML = reviews.map(r => {
    const dateStr = new Date(r.createdAt || Date.now()).toLocaleDateString('vi-VN', { month: 'long', year: 'numeric' });
    const thumb = r.images && r.images.length > 0 ? r.images[0] : null;

    return `
      <article class="review-card-item">
        <div style="display:flex; justify-content:space-between; align-items:flex-start; margin-bottom:10px;">
          <div>
            <a href="place-detail.html?id=${r.placeId}" style="font-size:1.15rem; font-weight:800; color:var(--text-main); display:block; margin-bottom:4px;">
              <i class="fa-solid fa-location-dot" style="color:var(--primary);"></i> ${r.placeName}
            </a>
            <div style="font-size:0.82rem; color:var(--text-muted);">${r.placeCategory || 'Địa điểm'} • ${r.placeProvince || 'Việt Nam'}</div>
          </div>
          <div style="font-size:0.8rem; color:var(--text-light);">${dateStr}</div>
        </div>

        <div style="margin-bottom:8px;">
          ${UI.renderBubbleRating(r.rating || 5, null, false)}
        </div>

        <h4 style="font-size:1.05rem; font-weight:800; color:var(--text-main); margin-bottom:6px;">"${r.title || 'Trải nghiệm tuyệt vời'}"</h4>
        <p style="font-size:0.92rem; color:var(--text-body); line-height:1.6; margin-bottom:12px;">${r.content}</p>

        ${thumb ? `
          <div style="margin-bottom:12px;">
            <img src="${thumb}" alt="Ảnh trải nghiệm" style="width:100%; max-height:280px; object-fit:cover; border-radius:var(--radius-md);" />
          </div>
        ` : ''}

        <div style="display:flex; justify-content:space-between; align-items:center; border-top:1px solid var(--border-subtle); padding-top:10px; font-size:0.82rem; color:var(--text-muted);">
          <span><i class="fa-solid fa-person-walking"></i> Chuyến đi: <strong>${r.travelerType || 'Du lịch'}</strong></span>
          <a href="place-detail.html?id=${r.placeId}" class="btn btn-outline btn-sm">Xem trang địa điểm <i class="fa-solid fa-arrow-right"></i></a>
        </div>
      </article>
    `;
  }).join('');
}

function loadVisitedPlaces(userId) {
  const container = document.getElementById('visitedPlacesGrid');
  if (!container) return;

  let allPlaces = [];
  if (typeof TravelData !== 'undefined') allPlaces = TravelData.getPlaces();

  if (allPlaces.length > 0) {
    container.innerHTML = allPlaces.slice(0, 3).map(p => UI.createPlaceCard(p)).join('');
  } else {
    container.innerHTML = '<p style="color:var(--text-muted);">Chưa có địa điểm đã ghé thăm.</p>';
  }
}

function switchProfileTab(tabName, btn) {
  document.querySelectorAll('.profile-tab-btn').forEach(b => b.classList.remove('active'));
  if (btn) btn.classList.add('active');

  document.getElementById('tab-pane-reviews').style.display = tabName === 'reviews' ? 'block' : 'none';
  document.getElementById('tab-pane-achievements').style.display = tabName === 'achievements' ? 'block' : 'none';
  document.getElementById('tab-pane-visited').style.display = tabName === 'visited' ? 'block' : 'none';
}
