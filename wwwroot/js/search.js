/* ==========================================================================
   MINIMAP & LANG THANG — TRIPADVISOR EXPLORE & SEARCH CONTROLLER
   ========================================================================== */

let allPlaces = [];
let filteredPlaces = [];
let selectedPlaceType = null;
let selectedPrice = null;
let selectedMinRating = null;
let viewMode = 'grid';
let debounceTimer = null;

document.addEventListener('DOMContentLoaded', () => {
  initSearchPage();
});

async function initSearchPage() {
  const headerEl = document.getElementById('site-header-mount');
  const footerEl = document.getElementById('site-footer-mount');
  if (headerEl) headerEl.innerHTML = UI.renderHeader('search');
  if (footerEl) footerEl.innerHTML = UI.renderFooter();

  await loadProvincesDropdown();
  await loadAllPlaces();
  parseUrlParams();
  applySearchFilters();
}

async function loadProvincesDropdown() {
  const select = document.getElementById('filter-province');
  if (!select) return;

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
    select.innerHTML = '<option value="">Tất cả tỉnh thành</option>' +
      provinces.map(p => `<option value="${p.name || p.id}">${p.name}</option>`).join('');
  }
}

async function loadAllPlaces() {
  try {
    if (typeof api !== 'undefined') {
      allPlaces = await api.getPlaces();
    }
  } catch(e) {}

  if (!allPlaces || allPlaces.length === 0) {
    if (typeof TravelData !== 'undefined') {
      allPlaces = TravelData.getPlaces();
    }
  }
}

function parseUrlParams() {
  const params = new URLSearchParams(window.location.search);
  const q = params.get('q');
  const prov = params.get('province');
  const type = params.get('type');
  const minRating = params.get('minRating');

  if (q) {
    const input = document.getElementById('search-keyword-input');
    if (input) input.value = q;
  }

  if (prov) {
    const provSelect = document.getElementById('filter-province');
    if (provSelect) {
      for (let i = 0; i < provSelect.options.length; i++) {
        if (provSelect.options[i].text.toLowerCase().includes(prov.toLowerCase()) ||
            provSelect.options[i].value.toLowerCase().includes(prov.toLowerCase())) {
          provSelect.selectedIndex = i;
          break;
        }
      }
    }
  }

  if (type) {
    selectedPlaceType = parseInt(type);
    const chip = document.querySelector(`.filter-chip-btn[data-type="${type}"]`);
    if (chip) chip.classList.add('active');
  }

  if (minRating) {
    selectedMinRating = parseFloat(minRating);
  }
}

function debounceSearch() {
  clearTimeout(debounceTimer);
  const input = document.getElementById('search-keyword-input');
  const clearBtn = document.getElementById('btn-clear-search');
  if (clearBtn) {
    clearBtn.style.display = input && input.value ? 'inline-block' : 'none';
  }
  debounceTimer = setTimeout(() => {
    applySearchFilters();
  }, 200);
}

function resetKeyword() {
  const input = document.getElementById('search-keyword-input');
  if (input) {
    input.value = '';
    debounceSearch();
  }
}

function toggleTypeFilter(typeId, btn) {
  if (selectedPlaceType === typeId) {
    selectedPlaceType = null;
    btn.classList.remove('active');
  } else {
    document.querySelectorAll('[data-type]').forEach(b => b.classList.remove('active'));
    selectedPlaceType = typeId;
    btn.classList.add('active');
  }
  applySearchFilters();
}

function togglePriceFilter(priceKey, btn) {
  if (selectedPrice === priceKey) {
    selectedPrice = null;
    btn.classList.remove('active');
  } else {
    document.querySelectorAll('.filter-chip-btn').forEach(b => {
      if (['$ Bình dân', '$$ Vừa phải', '$$$ Cao cấp'].some(t => b.innerText.includes(t))) {
        b.classList.remove('active');
      }
    });
    selectedPrice = priceKey;
    btn.classList.add('active');
  }
  applySearchFilters();
}

function toggleRatingFilter(ratingVal, btn) {
  if (selectedMinRating === ratingVal) {
    selectedMinRating = null;
    btn.classList.remove('active');
  } else {
    btn.parentElement.querySelectorAll('.filter-chip-btn').forEach(b => b.classList.remove('active'));
    selectedMinRating = ratingVal;
    btn.classList.add('active');
  }
  applySearchFilters();
}

function setViewMode(mode) {
  viewMode = mode;
  document.getElementById('btn-view-grid').classList.toggle('active', mode === 'grid');
  document.getElementById('btn-view-list').classList.toggle('active', mode === 'list');
  renderSearchResults();
}

function resetSearchFilters() {
  selectedPlaceType = null;
  selectedPrice = null;
  selectedMinRating = null;

  const kwInput = document.getElementById('search-keyword-input');
  if (kwInput) kwInput.value = '';

  const provSelect = document.getElementById('filter-province');
  if (provSelect) provSelect.selectedIndex = 0;

  const openNow = document.getElementById('check-open-now');
  if (openNow) openNow.checked = false;

  const tcOnly = document.getElementById('check-tc-only');
  if (tcOnly) tcOnly.checked = false;

  document.querySelectorAll('.filter-chip-btn').forEach(b => b.classList.remove('active'));

  applySearchFilters();
  UI.showToast('Đã xóa tất cả bộ lọc tìm kiếm', 'info');
}

function applySearchFilters() {
  const kw = (document.getElementById('search-keyword-input')?.value || '').trim().toLowerCase();
  const provVal = (document.getElementById('filter-province')?.value || '').toLowerCase();
  const tcOnly = document.getElementById('check-tc-only')?.checked || false;
  const sortVal = document.getElementById('sort-select')?.value || 'travelers_choice';

  filteredPlaces = allPlaces.filter(p => {
    // Keyword
    if (kw) {
      const matchName = (p.name || '').toLowerCase().includes(kw);
      const matchDesc = (p.description || '').toLowerCase().includes(kw);
      const matchAddr = (p.address || '').toLowerCase().includes(kw);
      const matchCat = (p.category?.name || p.categoryName || p.category || '').toLowerCase().includes(kw);
      if (!matchName && !matchDesc && !matchAddr && !matchCat) return false;
    }

    // Province
    if (provVal) {
      const pProv = (p.province?.name || p.provinceName || '').toLowerCase();
      if (!pProv.includes(provVal)) return false;
    }

    // Type
    if (selectedPlaceType) {
      const placeTypeId = p.placeTypeId || p.category?.placeTypeId || 1;
      if (placeTypeId !== selectedPlaceType) return false;
    }

    // Rating
    if (selectedMinRating) {
      const rating = Number(p.avgRating) || 5.0;
      if (rating < selectedMinRating) return false;
    }

    // Price
    if (selectedPrice) {
      const minP = p.minPrice || 0;
      if (selectedPrice === 'budget' && minP > 100000) return false;
      if (selectedPrice === 'mid' && (minP < 100000 || minP > 300000)) return false;
      if (selectedPrice === 'luxury' && minP < 300000) return false;
    }

    // Travelers Choice Only
    if (tcOnly) {
      const rating = Number(p.avgRating) || 0;
      if (rating < 4.5) return false;
    }

    return true;
  });

  // Sorting
  filteredPlaces.sort((a, b) => {
    if (sortVal === 'rating_desc') {
      return (Number(b.avgRating) || 0) - (Number(a.avgRating) || 0);
    } else if (sortVal === 'reviews_desc') {
      return (b.reviewCount || 0) - (a.reviewCount || 0);
    } else if (sortVal === 'price_asc') {
      return (a.minPrice || 0) - (b.minPrice || 0);
    } else if (sortVal === 'name_asc') {
      return (a.name || '').localeCompare(b.name || '');
    } else {
      // Travelers Choice default: combination of rating and review count
      const scoreA = (Number(a.avgRating) || 4.5) * 100 + (a.reviewCount || 0);
      const scoreB = (Number(b.avgRating) || 4.5) * 100 + (b.reviewCount || 0);
      return scoreB - scoreA;
    }
  });

  renderSearchResults();
}

function renderSearchResults() {
  const gridContainer = document.getElementById('search-results-grid');
  const listContainer = document.getElementById('search-results-list');
  const emptyState = document.getElementById('search-empty-state');
  const headingEl = document.getElementById('search-result-heading');
  const countEl = document.getElementById('search-result-count');

  const count = filteredPlaces.length;
  if (countEl) countEl.innerText = `${count} địa điểm`;
  if (headingEl) headingEl.innerText = `Kết quả khám phá (${count})`;

  if (count === 0) {
    if (gridContainer) gridContainer.style.display = 'none';
    if (listContainer) listContainer.style.display = 'none';
    if (emptyState) emptyState.style.display = 'block';
    return;
  }

  if (emptyState) emptyState.style.display = 'none';

  if (viewMode === 'grid') {
    if (gridContainer) {
      gridContainer.style.display = 'grid';
      gridContainer.innerHTML = filteredPlaces.map(p => UI.createPlaceCard(p)).join('');
    }
    if (listContainer) listContainer.style.display = 'none';
  } else {
    if (listContainer) {
      listContainer.style.display = 'block';
      listContainer.innerHTML = filteredPlaces.map((p, idx) => UI.createTripAdvisorListCard(p, idx + 1)).join('');
    }
    if (gridContainer) gridContainer.style.display = 'none';
  }
}
