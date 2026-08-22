import re
with open('D:/MiniMap/Services/AdminService.cs', 'r', encoding='utf-8') as f:
    content = f.read()

new_content = '''
        public async Task<List<PlaceProposalDto>> GetPendingProposalsAsync(long adminUserId, bool isSystemAdmin = false)
        {
            var query = _db.PlaceProposals
                .Include(p => p.Proposer)
                .Where(p => p.Status == "pending")
                .AsQueryable();

            if (!isSystemAdmin)
            {
                var catIds = await GetAdminCategoryIds(adminUserId, false);
                query = query.Where(p => catIds.Contains(p.CategoryId));
            }

            var list = await query.OrderByDescending(p => p.SubmittedAt).ToListAsync();

            return list.Select(p => new PlaceProposalDto
            {
                Id = p.Id,
                ProposalType = p.ProposalType,
                TargetPlaceId = p.TargetPlaceId,
                PlaceName = p.Name,
                Address = p.Address,
                ProposedBy = p.ProposedBy,
                ProposerName = p.Proposer?.FullName ?? "",
                Status = p.Status,
                SubmittedAt = p.SubmittedAt
            }).ToList();
        }

        public async Task<bool> ApproveProposalAsync(long proposalId, long adminUserId)
        {
            var proposal = await _db.PlaceProposals.Include(p => p.Media).FirstOrDefaultAsync(p => p.Id == proposalId);
            if (proposal == null) return false;

            if (proposal.ProposalType == "create")
            {
                var place = new Place
                {
                    Name = proposal.Name,
                    Description = proposal.Description,
                    Address = proposal.Address,
                    Phone = proposal.Phone,
                    Website = proposal.Website,
                    MinPrice = proposal.MinPrice,
                    MaxPrice = proposal.MaxPrice,
                    OpeningHours = proposal.OpeningHours,
                    Latitude = proposal.Latitude,
                    Longitude = proposal.Longitude,
                    ProvinceId = proposal.ProvinceId,
                    CategoryId = proposal.CategoryId,
                    Status = "active",
                    AvgRating = 0,
                    ReviewCount = 0
                };
                _db.Places.Add(place);
                await _db.SaveChangesAsync();

                foreach (var m in proposal.Media)
                {
                    _db.PlaceMedia.Add(new PlaceMedia
                    {
                        PlaceId = place.Id,
                        MediaType = m.MediaType,
                        Url = m.Url,
                        DisplayOrder = m.DisplayOrder,
                        UploadedBy = proposal.ProposedBy
                    });
                }
                proposal.ApprovedPlaceId = place.Id;
            }
            else if (proposal.ProposalType == "edit" && proposal.TargetPlaceId != null)
            {
                var place = await _db.Places.FindAsync(proposal.TargetPlaceId);
                if (place != null)
                {
                    place.Name = proposal.Name;
                    place.Description = proposal.Description;
                    place.Address = proposal.Address;
                    place.Phone = proposal.Phone;
                    place.Website = proposal.Website;
                    place.MinPrice = proposal.MinPrice;
                    place.MaxPrice = proposal.MaxPrice;
                    place.OpeningHours = proposal.OpeningHours;
                    if (proposal.Latitude.HasValue) place.Latitude = proposal.Latitude;
                    if (proposal.Longitude.HasValue) place.Longitude = proposal.Longitude;
                    place.ProvinceId = proposal.ProvinceId;
                    place.CategoryId = proposal.CategoryId;
                    place.UpdatedAt = DateTime.UtcNow;

                    foreach (var m in proposal.Media)
                    {
                        _db.PlaceMedia.Add(new PlaceMedia
                        {
                            PlaceId = place.Id,
                            MediaType = m.MediaType,
                            Url = m.Url,
                            DisplayOrder = m.DisplayOrder,
                            UploadedBy = proposal.ProposedBy
                        });
                    }
                }
            }

            proposal.Status = "approved";
            proposal.ReviewedBy = adminUserId;
            proposal.ReviewedAt = DateTime.UtcNow;

            _db.Notifications.Add(new Notification
            {
                UserId = proposal.ProposedBy,
                Content = $"Đề xuất {(proposal.ProposalType == "create" ? "tạo mới" : "chỉnh sửa")} địa điểm '{proposal.Name}' của bạn đã được phê duyệt!",
                Type = "place_approved",
                TargetType = "place",
                TargetId = proposal.ApprovedPlaceId ?? proposal.TargetPlaceId ?? 0
            });

            await AddAuditLogAsync(adminUserId, "APPROVE_PROPOSAL", "place_proposal", proposalId, $"Duyệt đề xuất: {proposal.Name}");
            await _db.SaveChangesAsync();
            return true;
        }

        public async Task<bool> RejectProposalAsync(long proposalId, long adminUserId, string reason)
        {
            var proposal = await _db.PlaceProposals.FindAsync(proposalId);
            if (proposal == null) return false;

            proposal.Status = "rejected";
            proposal.RejectReason = reason;
            proposal.ReviewedBy = adminUserId;
            proposal.ReviewedAt = DateTime.UtcNow;

            _db.Notifications.Add(new Notification
            {
                UserId = proposal.ProposedBy,
                Content = $"Đề xuất {(proposal.ProposalType == "create" ? "tạo mới" : "chỉnh sửa")} địa điểm '{proposal.Name}' bị từ chối. Lý do: {reason}",
                Type = "place_rejected",
                TargetType = "place_proposal",
                TargetId = proposal.Id
            });

            await AddAuditLogAsync(adminUserId, "REJECT_PROPOSAL", "place_proposal", proposalId, $"Từ chối đề xuất: {proposal.Name}. Lý do: {reason}");
            await _db.SaveChangesAsync();
            return true;
        }
'''

start_idx = content.find('public async Task<List<PlaceDto>> GetPendingPlacesForAdminAsync')
end_idx = content.find('public async Task<List<ReportDto>> GetReportsForAdminAsync')

if start_idx != -1 and end_idx != -1:
    content = content[:start_idx] + new_content.strip() + '\n\n        ' + content[end_idx:]
    with open('D:/MiniMap/Services/AdminService.cs', 'w', encoding='utf-8') as f:
        f.write(content)
    print('Replaced successfully')
else:
    print('Failed to find start or end index')
