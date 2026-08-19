/* ==========================================================================
   MINIMAP & LANG THANG — HOMEPAGE & GLOBAL APP CONTROLLER (TripAdvisor UI)
   ========================================================================== */

document.addEventListener('DOMContentLoaded', () => {
  initHomepage();
});

async function initHomepage() {
  renderHeaderAndFooter('home');
  
  try {
    await Promise.all([
      renderFeaturedPlaces(),
      renderDestinations(),
      renderSpecialtyFoods(),
      renderTopRatedPlaces(),
      setupHeroSearch()
    ]);
  } catch (err) {
    console.error("Error loading homepage data:", err);
  }
}

function renderHeaderAndFooter(activePage) {
  const headerEl = document.getElementById('site-header-mount');
  const footerEl = document.getElementById('site-footer-mount');

  if (headerEl) headerEl.innerHTML = UI.renderHeader(activePage);
  if (footerEl) footerEl.innerHTML = UI.renderFooter();

  // Header scroll shadow effect
  window.addEventListener('scroll', () => {
    const header = document.querySelector('.site-header');
    if (header) {
      header.classList.toggle('scrolled', window.scrollY > 20);
    }
  });
}

async function setupHeroSearch() {
  const provSelect = document.getElementById('hero-province-select');
  if (!provSelect) return;

  let provinces = [];
  try {
    if (typeof api !== 'undefined') {
      provinces = await api.getProvinces();
    }
  } catch(e) {}

  if (!provinces || provinces.length === 0) {
    if (typeof TravelData !== 'undefined') {
      provinces = TravelData.getProvinces();
    }
  }

  if (provinces && provinces.length > 0) {
    provSelect.innerHTML = '<option value="">Toàn quốc</option>' +
      provinces.map(p => `<option value="${p.name || p.id}">${p.name}</option>`).join('');
  }
}

async function renderFeaturedPlaces() {
  const container = document.getElementById('featured-places-grid');
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

  if (places && places.length > 0) {
    const featured = places.slice(0, 8);
    container.innerHTML = featured.map(p => UI.createPlaceCard(p)).join('');
  } else {
    container.innerHTML = '<p style="grid-column:1/-1; text-align:center; color:var(--text-muted);">Đang cập nhật địa điểm...</p>';
  }
}

async function renderDestinations() {
  const container = document.getElementById('destinations-grid');
  if (!container) return;
  
  let provinces = [];
  try {
    if (typeof api !== 'undefined') {
      provinces = await api.getProvinces();
    }
  } catch(e) {}

  if (!provinces || provinces.length === 0) {
    if (typeof TravelData !== 'undefined') {
      provinces = TravelData.getProvinces();
    }
  }

  const destinationImages = [
    { name: "TP. Hồ Chí Minh", img: "https://images.unsplash.com/photo-1583417319070-4a69db38a482?w=600", count: 480 },
    { name: "Hà Nội", img: "https://images.unsplash.com/photo-1509042239860-f550ce710b93?w=600", count: 320 },
    { name: "Đà Nẵng", img: "https://images.unsplash.com/photo-1559592413-7cec4d0cae2b?w=600", count: 215 },
    { name: "Lâm Đồng (Đà Lạt)", img: "https://images.unsplash.com/photo-1514432324607-a09d9b4aefdd?w=600", count: 190 },
    { name: "Kiên Giang (Phú Quốc)", img: "https://images.unsplash.com/photo-1540555700478-4be289fbecef?w=600", count: 145 },
    { name: "Quảng Ninh (Hạ Long)", img: "https://images.unsplash.com/photo-1528127269322-539801943592?w=600", count: 160 }
  ];

  container.innerHTML = destinationImages.map(d => `
    <a href="pages/search.html?province=${encodeURIComponent(d.name.split('(')[0].trim())}" class="destination-card">
      <img src="${d.img}" alt="${d.name}" class="destination-card-img" loading="lazy" />
      <div class="destination-card-overlay">
        <div class="destination-card-title">${d.name}</div>
        <div class="destination-card-count"><i class="fa-solid fa-location-dot"></i> ${d.count} địa điểm khám phá</div>
      </div>
    </a>
  `).join('');
}

async function renderSpecialtyFoods() {
  const container = document.getElementById('specialty-foods-grid');
  if (!container) return;

  let foods = [];
  try {
    if (typeof api !== 'undefined') {
      foods = await api.request('/foods');
    }
  } catch(e) {}

  if (!foods || foods.length === 0) {
    if (typeof TravelData !== 'undefined') {
      foods = TravelData.getFoods();
    }
  }

  if (foods && foods.length > 0) {
    const topFoods = foods.slice(0, 3);
    container.innerHTML = topFoods.map(f => {
      const provName = f.foodProvinces && f.foodProvinces.length > 0
        ? (f.foodProvinces[0].province?.name || f.foodProvinces[0].provinceName || 'Việt Nam')
        : (f.provinceName || 'Việt Nam');
      const img = f.imageUrl || f.image || 'https://images.unsplash.com/photo-1582878826629-29b7ad1cdc43?w=600';
      return `
        <div class="place-card" style="display:flex; flex-direction:row; align-items:stretch;">
          <div style="width:140px; position:relative; flex-shrink:0;">
            <img src="${img}" alt="${f.name}" style="width:100%; height:100%; object-fit:cover;" loading="lazy" />
          </div>
          <div style="padding:16px; display:flex; flex-direction:column; flex:1;">
            <span class="badge badge-category" style="align-self:flex-start; margin-bottom:4px;"><i class="fa-solid fa-bowl-rice"></i> Đặc sản ${provName}</span>
            <h3 style="font-size:1.05rem; font-weight:800; color:var(--text-main); margin-bottom:6px;">${f.name}</h3>
            <p style="font-size:0.82rem; color:var(--text-muted); line-height:1.4; margin-bottom:10px; display:-webkit-box; -webkit-line-clamp:2; -webkit-box-orient:vertical; overflow:hidden;">${f.description || 'Hương vị truyền thống đặc sắc'}</p>
            <a href="pages/foods.html?id=${f.id}" class="btn btn-outline btn-sm" style="margin-top:auto; align-self:flex-start; padding:4px 12px; font-size:0.78rem;">
              <i class="fa-solid fa-utensils"></i> Xem nơi bán ngon
            </a>
          </div>
        </div>
      `;
    }).join('');
  }
}

async function renderTopRatedPlaces() {
  const container = document.getElementById('top-rated-places-grid');
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

  if (places && places.length > 0) {
    const topPlaces = [...places]
      .sort((a, b) => (Number(b.avgRating) || 0) - (Number(a.avgRating) || 0))
      .slice(0, 4);

    container.innerHTML = topPlaces.map(p => UI.createPlaceCard(p)).join('');
  }
}
