/* ==========================================================================
   TRAVELREVIEW - REVIEW SUBMISSION & RATING LOGIC
   ========================================================================== */

let selectedStarRating = 5;

document.addEventListener('DOMContentLoaded', () => {
  initReviewStars();
});

function initReviewStars() {
  const starContainer = document.getElementById('review-star-picker');
  if (!starContainer) return;

  const stars = starContainer.querySelectorAll('i');
  stars.forEach((star, idx) => {
    star.addEventListener('click', () => {
      setStarRating(idx + 1);
    });
  });
}

function setStarRating(rating) {
  selectedStarRating = rating;
  const starContainer = document.getElementById('review-star-picker');
  if (!starContainer) return;

  const stars = starContainer.querySelectorAll('i');
  stars.forEach((star, idx) => {
    if (idx < rating) {
      star.className = 'fa-solid fa-star';
      star.style.color = 'var(--amber)';
    } else {
      star.className = 'fa-regular fa-star';
      star.style.color = 'var(--text-light)';
    }
  });

  const ratingLabel = document.getElementById('review-rating-label');
  const labels = ['', '1 sao - Kém', '2 sao - Tạm được', '3 sao - Bình thường', '4 sao - Tốt', '5 sao - Tuyệt vời'];
  if (ratingLabel) ratingLabel.innerText = labels[rating];
}

function openWriteReviewModal() {
  setStarRating(5);
  document.getElementById('review-content-input').value = '';
  document.getElementById('review-image-input').value = '';
  document.getElementById('review-video-input').value = '';
  UI.openModal('modal-write-review');
}

function submitNewReview() {
  const content = document.getElementById('review-content-input')?.value.trim();
  const imgUrl = document.getElementById('review-image-input')?.value.trim();
  const videoUrl = document.getElementById('review-video-input')?.value.trim();

  if (!content) {
    UI.showToast('Vui lòng nhập nội dung đánh giá của bạn!', 'error');
    return;
  }

  if (!currentPlace) return;

  const reviewData = {
    placeId: currentPlace.id,
    rating: selectedStarRating,
    content: content,
    images: imgUrl ? [imgUrl] : [],
    videoUrl: videoUrl || ''
  };

  TravelData.addReview(reviewData);
  UI.closeModal('modal-write-review');
  UI.showToast('Đánh giá của bạn đã được đăng thành công!', 'success');

  // Re-render reviews & updated ratings on page
  currentPlace = TravelData.getPlaceById(currentPlace.id);
  renderPlaceHeader();
  renderPlaceReviews();
}
