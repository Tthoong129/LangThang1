/* ==========================================================================
   TRAVELREVIEW - DATA LAYER & LOCAL STORAGE REPOSITORY
   Contains realistic Vietnamese Travel, Dining, Stay & Entertainment Mock Data
   ========================================================================== */

const TravelData = (() => {
  // Key Storage Names
  const STORAGE_KEYS = {
    PLACES: 'tr_places',
    FOODS: 'tr_foods',
    REVIEWS: 'tr_reviews',
    FAVORITES: 'tr_favorites',
    VISIT_LOGS: 'tr_visit_logs',
    HISTORY: 'tr_history',
    PROPOSALS: 'tr_proposals',
    EDIT_PROPOSALS: 'tr_edit_proposals',
    REPORTS: 'tr_reports',
    APPEALS: 'tr_appeals',
    NOTIFICATIONS: 'tr_notifications',
    USER: 'tr_user_profile'
  };

  // Regions & Provinces Data
  const regions = [
    { id: 1, name: 'Miền Bắc' },
    { id: 2, name: 'Miền Trung' },
    { id: 3, name: 'Miền Nam' }
  ];

  const provinces = [
    { id: 1, name: 'Hà Nội', regionId: 1, image: 'https://images.unsplash.com/photo-1509042239860-f550ce710b93?w=500', placeCount: 142 },
    { id: 2, name: 'Lào Cai (Sa Pa)', regionId: 1, image: 'https://images.unsplash.com/photo-1544085311-11a028465b03?w=500', placeCount: 68 },
    { id: 3, name: 'Quảng Ninh (Hạ Long)', regionId: 1, image: 'https://images.unsplash.com/photo-1528127269322-539801943592?w=500', placeCount: 95 },
    { id: 4, name: 'Đà Nẵng', regionId: 2, image: 'https://images.unsplash.com/photo-1559592413-7cec4d0cae2b?w=500', placeCount: 185 },
    { id: 5, name: 'Thừa Thiên Huế', regionId: 2, image: 'https://images.unsplash.com/photo-1569154941061-e231b4725ef1?w=500', placeCount: 76 },
    { id: 6, name: 'Khánh Hòa (Nha Trang)', regionId: 2, image: 'https://images.unsplash.com/photo-1507525428034-b723cf961d3e?w=500', placeCount: 120 },
    { id: 7, name: 'Lâm Đồng (Đà Lạt)', regionId: 2, image: 'https://images.unsplash.com/photo-1514432324607-a09d9b4aefdd?w=500', placeCount: 160 },
    { id: 8, name: 'TP. Hồ Chí Minh', regionId: 3, image: 'https://images.unsplash.com/photo-1583417319070-4a69db38a482?w=500', placeCount: 230 },
    { id: 9, name: 'Kiên Giang (Phú Quốc)', regionId: 3, image: 'https://images.unsplash.com/photo-1540555700478-4be289fbecef?w=500', placeCount: 110 },
    { id: 10, name: 'Bà Rịa - Vũng Tàu', regionId: 3, image: 'https://images.unsplash.com/photo-1506744038136-46273834b3fb?w=500', placeCount: 88 }
  ];

  // Place Types & Categories
  const placeTypes = [
    { id: 1, name: 'Ăn uống', icon: 'fa-utensils' },
    { id: 2, name: 'Du lịch', icon: 'fa-umbrella-beach' },
    { id: 3, name: 'Lưu trú', icon: 'fa-hotel' },
    { id: 4, name: 'Vui chơi', icon: 'fa-gamepad' }
  ];

  const categories = [
    { id: 1, placeTypeId: 1, name: 'Nhà hàng & Quán ăn' },
    { id: 2, placeTypeId: 1, name: 'Quán Cà phê & Trà' },
    { id: 3, placeTypeId: 1, name: 'Ẩm thực đường phố' },
    { id: 4, placeTypeId: 2, name: 'Bãi biển & Đảo' },
    { id: 5, placeTypeId: 2, name: 'Núi, Thác & Thiên nhiên' },
    { id: 6, placeTypeId: 2, name: 'Di tích & Danh lam thắng cảnh' },
    { id: 7, placeTypeId: 3, name: 'Khách sạn cao cấp' },
    { id: 8, placeTypeId: 3, name: 'Resort nghỉ dưỡng' },
    { id: 9, placeTypeId: 3, name: 'Homestay & Villa' },
    { id: 10, placeTypeId: 4, name: 'Công viên & Khu giải trí' },
    { id: 11, placeTypeId: 4, name: 'Rạp phim & Trải nghiệm đêm' }
  ];

  // Initial Mock Places
  const defaultPlaces = [
    {
      id: 1,
      name: 'Phở Thìn Lò Đúc',
      category: 'Nhà hàng & Quán ăn',
      placeTypeId: 1,
      provinceId: 1,
      provinceName: 'Hà Nội',
      address: '13 Lò Đúc, Phạm Đình Hổ, Hai Bà Trưng, Hà Nội',
      phone: '024 3943 4455',
      website: 'https://phothin.vn',
      minPrice: 65000,
      maxPrice: 110000,
      openingHours: '06:00 - 21:00',
      avgRating: 4.8,
      reviewCount: 142,
      latitude: 21.018281,
      longitude: 105.856985,
      images: [
        'https://images.unsplash.com/photo-1582878826629-29b7ad1cdc43?w=800',
        'https://images.unsplash.com/photo-1569718212165-3a8278d5f624?w=800',
        'https://images.unsplash.com/photo-1503764654157-72d979d9af2f?w=800',
        'https://images.unsplash.com/photo-1555396273-367ea4eb4db5?w=800',
        'https://images.unsplash.com/photo-1517248135467-4c7edcad34c4?w=800'
      ],
      description: 'Quán phở bò xào lăn trứ danh Hà Nội từ năm 1979 với nước dùng béo ngậy đậm đà, hành lá ngập bát và thịt bò mềm thơm được đảo trên chảo lửa lớn.',
      foodsServed: ['Phở Bò Tái Lăn', 'Quẩy Giòn Hà Nội'],
      isFeatured: true
    },
    {
      id: 2,
      name: 'Cà Phê Giảng (Cà Phê Trứng Cổ Hà Nội)',
      category: 'Quán Cà phê & Trà',
      placeTypeId: 1,
      provinceId: 1,
      provinceName: 'Hà Nội',
      address: '39 Nguyễn Hữu Huân, Hàng Bạc, Hoàn Kiếm, Hà Nội',
      phone: '098 989 2298',
      website: '',
      minPrice: 35000,
      maxPrice: 60000,
      openingHours: '07:00 - 22:30',
      avgRating: 4.9,
      reviewCount: 238,
      latitude: 21.033671,
      longitude: 105.854341,
      images: [
        'https://images.unsplash.com/photo-1501339847302-ac426a4a7cbb?w=800',
        'https://images.unsplash.com/photo-1514432324607-a09d9b4aefdd?w=800',
        'https://images.unsplash.com/photo-1554118811-1e0d58224f24?w=800',
        'https://images.unsplash.com/photo-1495474472287-4d71bcdd2085?w=800',
        'https://images.unsplash.com/photo-1442512595331-e89e73853f31?w=800'
      ],
      description: 'Nơi khai sinh ra món Cà phê trứng nức tiếng từ những năm 1946 do cụ Nguyễn Văn Giảng sáng tạo. Lớp kem trứng sánh mịn béo ngậy kết hợp hoàn hảo cùng hương vị Robusta nồng nàn.',
      foodsServed: ['Cà Phê Trứng', 'Cacao Trứng'],
      isFeatured: true
    },
    {
      id: 3,
      name: 'Bà Nà Hills & Cầu Vàng',
      category: 'Công viên & Khu giải trí',
      placeTypeId: 4,
      provinceId: 4,
      provinceName: 'Đà Nẵng',
      address: 'Thôn An Sơn, Xã Hòa Ninh, Huyện Hòa Vang, Đà Nẵng',
      phone: '0236 3791 999',
      website: 'https://banahills.sunworld.vn',
      minPrice: 650000,
      maxPrice: 1250000,
      openingHours: '07:30 - 21:30',
      avgRating: 4.9,
      reviewCount: 512,
      latitude: 15.998967,
      longitude: 107.986612,
      images: [
        'https://images.unsplash.com/photo-1559592413-7cec4d0cae2b?w=800',
        'https://images.unsplash.com/photo-1570789210967-2cac24afeb00?w=800',
        'https://images.unsplash.com/photo-1528127269322-539801943592?w=800',
        'https://images.unsplash.com/photo-1507525428034-b723cf961d3e?w=800',
        'https://images.unsplash.com/photo-1566073771259-6a8506099945?w=800'
      ],
      description: 'Quần thể du lịch nghỉ dưỡng trên đỉnh núi Chúa ở độ cao 1.487m. Nổi tiếng với Cầu Vàng - kiệt tác kiến trúc được nâng đỡ bởi bàn tay khổng lồ và Làng Pháp cổ kính thơ mộng.',
      foodsServed: ['Buffet Quốc tế Beer Plaza'],
      isFeatured: true
    },
    {
      id: 4,
      name: 'InterContinental Danang Sun Peninsula Resort',
      category: 'Resort nghỉ dưỡng',
      placeTypeId: 3,
      provinceId: 4,
      provinceName: 'Đà Nẵng',
      address: 'Bãi Bắc, Bán đảo Sơn Trà, Đà Nẵng',
      phone: '0236 3938 888',
      website: 'https://danang.intercontinental.com',
      minPrice: 8500000,
      maxPrice: 38000000,
      openingHours: '24/7',
      avgRating: 5.0,
      reviewCount: 186,
      latitude: 16.120712,
      longitude: 108.310156,
      images: [
        'https://images.unsplash.com/photo-1566073771259-6a8506099945?w=800',
        'https://images.unsplash.com/photo-1582719508461-905c673771fd?w=800',
        'https://images.unsplash.com/photo-1540555700478-4be289fbecef?w=800',
        'https://images.unsplash.com/photo-1507525428034-b723cf961d3e?w=800',
        'https://images.unsplash.com/photo-1519046904884-53103b34b206?w=800'
      ],
      description: 'Khu nghỉ dưỡng 5 sao đẳng cấp thế giới do kiến trúc sư lừng danh Bill Bensley thiết kế, trải dài trên sườn đồi nhìn thẳng ra vịnh biển riêng tư nguyên sơ tại bán đảo Sơn Trà.',
      foodsServed: ['Hải sản tươi sống Sơn Trà', 'La Maison 1888'],
      isFeatured: true
    },
    {
      id: 5,
      name: 'Quán Bụi - Hương Vị Quê Nhà Sài Gòn',
      category: 'Nhà hàng & Quán ăn',
      placeTypeId: 1,
      provinceId: 8,
      provinceName: 'TP. Hồ Chí Minh',
      address: '19 Ngô Văn Năm, Bến Nghé, Quận 1, TP. Hồ Chí Minh',
      phone: '028 3829 1515',
      website: '',
      minPrice: 120000,
      maxPrice: 450000,
      openingHours: '07:00 - 23:00',
      avgRating: 4.7,
      reviewCount: 98,
      latitude: 10.780183,
      longitude: 106.705298,
      images: [
        'https://images.unsplash.com/photo-1555396273-367ea4eb4db5?w=800',
        'https://images.unsplash.com/photo-1517248135467-4c7edcad34c4?w=800',
        'https://images.unsplash.com/photo-1582878826629-29b7ad1cdc43?w=800',
        'https://images.unsplash.com/photo-1569718212165-3a8278d5f624?w=800',
        'https://images.unsplash.com/photo-1503764654157-72d979d9af2f?w=800'
      ],
      description: 'Không gian ẩm thực Việt truyền thống ấm cúng giữa lòng Quận 1 với các món ăn dân dã ba miền chuẩn vị: Cá kho tộ, canh chua cá lóc, thịt kho hột vịt, rau luộc chấm kho quẹt.',
      foodsServed: ['Cá Kho Tộ', 'Canh Chua Nam Bộ', 'Cơm Tấm'],
      isFeatured: false
    },
    {
      id: 6,
      name: 'Bãi Sao Phú Quốc',
      category: 'Bãi biển & Đảo',
      placeTypeId: 2,
      provinceId: 9,
      provinceName: 'Kiên Giang (Phú Quốc)',
      address: 'Ấp 4, Phường An Thới, TP. Phú Quốc, Kiên Giang',
      phone: '',
      website: '',
      minPrice: 0,
      maxPrice: 200000,
      openingHours: '06:00 - 18:30',
      avgRating: 4.8,
      reviewCount: 310,
      latitude: 10.054366,
      longitude: 104.032655,
      images: [
        'https://images.unsplash.com/photo-1507525428034-b723cf961d3e?w=800',
        'https://images.unsplash.com/photo-1519046904884-53103b34b206?w=800',
        'https://images.unsplash.com/photo-1540555700478-4be289fbecef?w=800',
        'https://images.unsplash.com/photo-1566073771259-6a8506099945?w=800',
        'https://images.unsplash.com/photo-1582719508461-905c673771fd?w=800'
      ],
      description: 'Bãi biển cát trắng mịn như kem trải dài hơn 7km ôm trọn làn nước biển xanh ngọc bích êm đềm, rợp bóng dừa nghiêng thơ mộng.',
      foodsServed: ['Gỏi Cá Trích Phú Quốc', 'Nhum Biển Nướng Mỡ Hành'],
      isFeatured: true
    },
    {
      id: 7,
      name: 'Mê Linh Coffee Garden Đà Lạt',
      category: 'Quán Cà phê & Trà',
      placeTypeId: 1,
      provinceId: 7,
      provinceName: 'Lâm Đồng (Đà Lạt)',
      address: 'Tổ 20, Thôn 4, Xã Tà Nung, TP. Đà Lạt, Lâm Đồng',
      phone: '091 961 9888',
      website: '',
      minPrice: 45000,
      maxPrice: 120000,
      openingHours: '07:00 - 18:00',
      avgRating: 4.6,
      reviewCount: 165,
      latitude: 11.90562,
      longitude: 108.35123,
      images: [
        'https://images.unsplash.com/photo-1514432324607-a09d9b4aefdd?w=800',
        'https://images.unsplash.com/photo-1501339847302-ac426a4a7cbb?w=800',
        'https://images.unsplash.com/photo-1554118811-1e0d58224f24?w=800',
        'https://images.unsplash.com/photo-1495474472287-4d71bcdd2085?w=800',
        'https://images.unsplash.com/photo-1442512595331-e89e73853f31?w=800'
      ],
      description: 'Quán cà phê với góc nhìn 360 độ ngắm trọn thung lũng đồi chè và cánh đồng hoa bạt ngàn của phố núi Đà Lạt. Thưởng thức hương vị cà phê chồn nguyên chất đúng điệu.',
      foodsServed: ['Cà Phê Chồn', 'Bánh ngọt Đà Lạt'],
      isFeatured: false
    },
    {
      id: 8,
      name: 'Fansipan Legend & Đỉnh Nóc Nhà Đông Dương',
      category: 'Núi, Thác & Thiên nhiên',
      placeTypeId: 2,
      provinceId: 2,
      provinceName: 'Lào Cai (Sa Pa)',
      address: 'Đường Nguyễn Chí Thanh, Thị xã Sa Pa, Lào Cai',
      phone: '0214 3818 888',
      website: 'https://fansipanlegend.sunworld.vn',
      minPrice: 750000,
      maxPrice: 1100000,
      openingHours: '07:30 - 17:30',
      avgRating: 4.9,
      reviewCount: 420,
      latitude: 22.303378,
      longitude: 103.775317,
      images: [
        'https://images.unsplash.com/photo-1544085311-11a028465b03?w=800',
        'https://images.unsplash.com/photo-1528127269322-539801943592?w=800',
        'https://images.unsplash.com/photo-1559592413-7cec4d0cae2b?w=800',
        'https://images.unsplash.com/photo-1570789210967-2cac24afeb00?w=800',
        'https://images.unsplash.com/photo-1566073771259-6a8506099945?w=800'
      ],
      description: 'Chinh phục đỉnh núi cao nhất bán đảo Đông Dương (3.143m) qua hệ thống cáp treo ba dây kỷ lục thế giới, ngắm biển mây bồng bềnh và quần thể tâm linh kỳ vĩ.',
      foodsServed: ['Lẩu Cá Tầm Sa Pa', 'Thắng Cố Bản Địa'],
      isFeatured: true
    },
    {
      id: 9,
      name: 'Bún Bò Huế Bà Tuyết',
      category: 'Nhà hàng & Quán ăn',
      placeTypeId: 1,
      provinceId: 5,
      provinceName: 'Thừa Thiên Huế',
      address: '47 Nguyễn Công Trứ, Phú Hội, TP. Huế',
      phone: '0234 3822 555',
      website: '',
      minPrice: 40000,
      maxPrice: 65000,
      openingHours: '06:00 - 21:00',
      avgRating: 4.8,
      reviewCount: 178,
      latitude: 16.4674,
      longitude: 107.5905,
      images: [
        'https://images.unsplash.com/photo-1582878826629-29b7ad1cdc43?w=800',
        'https://images.unsplash.com/photo-1569718212165-3a8278d5f624?w=800',
        'https://images.unsplash.com/photo-1503764654157-72d979d9af2f?w=800',
        'https://images.unsplash.com/photo-1555396273-367ea4eb4db5?w=800',
        'https://images.unsplash.com/photo-1517248135467-4c7edcad34c4?w=800'
      ],
      description: 'Hương vị bún bò Huế gia truyền đậm đà mùi mắm ruốc và sả thơm nức. Bát bún đầy đặn với chả cua béo ngậy, nạm bò mềm thơm và móng giò giòn sần sật.',
      foodsServed: ['Bún Bò Huế', 'Chả Cua Huế'],
      isFeatured: false
    },
    {
      id: 10,
      name: 'Vịnh Hạ Long & Du Thuyền Paradise',
      category: 'Bãi biển & Đảo',
      placeTypeId: 2,
      provinceId: 3,
      provinceName: 'Quảng Ninh (Hạ Long)',
      address: 'Cảng Tuần Châu, TP. Hạ Long, Quảng Ninh',
      phone: '0203 3842 360',
      website: 'https://paradisevietnam.com',
      minPrice: 3200000,
      maxPrice: 12000000,
      openingHours: '24/7',
      avgRating: 4.9,
      reviewCount: 388,
      latitude: 20.9167,
      longitude: 107.0333,
      images: [
        'https://images.unsplash.com/photo-1528127269322-539801943592?w=800',
        'https://images.unsplash.com/photo-1507525428034-b723cf961d3e?w=800',
        'https://images.unsplash.com/photo-1559592413-7cec4d0cae2b?w=800',
        'https://images.unsplash.com/photo-1570789210967-2cac24afeb00?w=800',
        'https://images.unsplash.com/photo-1566073771259-6a8506099945?w=800'
      ],
      description: 'Di sản thiên nhiên thế giới UNESCO với hàng nghìn hòn đảo đá vôi kỳ vĩ nhấp nhô trên làn nước biển xanh biếc. Trải nghiệm du thuyền 5 sao đẳng cấp.',
      foodsServed: ['Chả Mực Hạ Long', 'Hải Sản Tươi Sống Vịnh Bắc Bộ'],
      isFeatured: true
    }
  ];

  // Specialty Foods
  const defaultFoods = [
    {
      id: 1,
      name: 'Phở Bò Hà Nội',
      province: 'Hà Nội',
      provinceId: 1,
      image: 'https://images.unsplash.com/photo-1582878826629-29b7ad1cdc43?w=600',
      description: 'Món ăn quốc hồn quốc túy với bánh phở dẻo mềm, thịt bò tươi thái mỏng, nước dùng ninh từ xương bò và thảo mộc quế hồi thơm lừng.',
      placesServingIds: [1]
    },
    {
      id: 2,
      name: 'Cà Phê Trứng',
      province: 'Hà Nội',
      provinceId: 1,
      image: 'https://images.unsplash.com/photo-1514432324607-a09d9b4aefdd?w=600',
      description: 'Thức uống sáng tạo từ lòng đỏ trứng gà tươi đánh bông mịn béo ngậy cùng cà phê phin nguyên chất thơm đượm nồng nàn.',
      placesServingIds: [2]
    },
    {
      id: 3,
      name: 'Mì Quảng Đà Nẵng',
      province: 'Đà Nẵng',
      provinceId: 4,
      image: 'https://images.unsplash.com/photo-1617093727343-374698b1b08d?w=600',
      description: 'Mì sợi dai vàng nghệ dùng kèm tôm, thịt heo, trứng cút, bánh tráng nướng giòn rụm, đậu phộng rang và rau sống tươi xanh.',
      placesServingIds: [3, 4]
    },
    {
      id: 4,
      name: 'Gỏi Cá Trích Phú Quốc',
      province: 'Kiên Giang (Phú Quốc)',
      provinceId: 9,
      image: 'https://images.unsplash.com/photo-1555396273-367ea4eb4db5?w=600',
      description: 'Món gỏi đặc sản từ thịt cá trích tươi rói vừa đánh bắt, trộn cùng dừa nạo, hành tây, ớt, cuốn bánh tráng chấm nước mắm nhĩ Phú Quốc đậm đà.',
      placesServingIds: [6]
    },
    {
      id: 5,
      name: 'Cơm Tấm Sườn Bì Chả Sài Gòn',
      province: 'TP. Hồ Chí Minh',
      provinceId: 8,
      image: 'https://images.unsplash.com/photo-1627308595229-7830a5c91f9f?w=600',
      description: 'Hạt cơm tấm thơm bùi ăn cùng sườn nướng mỡ hành cháy cạnh xém thơm, chả trứng, bì giòn và chén nước mắm chua ngọt chuẩn vị Sài Gòn.',
      placesServingIds: [5]
    }
  ];

  // Reviews Data
  const defaultReviews = [
    {
      id: 1,
      placeId: 1,
      userId: 101,
      userName: 'Nguyễn Văn An',
      userAvatar: 'https://images.unsplash.com/photo-1535713875002-d1d0cf377fde?w=150',
      rating: 5,
      content: 'Phở xào lăn ở đây vị nước dùng béo ngậy đặc trưng không nơi nào có được. Thịt bò mềm ngọt và hành lá thơm nức!',
      images: ['https://images.unsplash.com/photo-1582878826629-29b7ad1cdc43?w=500'],
      videoUrl: '',
      createdAt: '2026-08-10T09:30:00Z',
      comments: [
        { id: 1, userName: 'Trần Thị Mai', content: 'Ăn kèm thêm đĩa quẩy giòn nữa là tuyệt hảo bạn nhé!', createdAt: '2026-08-10T10:15:00Z' }
      ]
    },
    {
      id: 2,
      placeId: 2,
      userId: 102,
      userName: 'Trần Thị Mai',
      userAvatar: 'https://images.unsplash.com/photo-1494790108377-be9c29b29330?w=150',
      rating: 5,
      content: 'Cà phê trứng sánh mịn như bơ kem, thơm lừng và hoàn toàn không bị tanh mùi trứng. Không gian phố cổ xưa rất chill.',
      images: ['https://images.unsplash.com/photo-1501339847302-ac426a4a7cbb?w=500'],
      videoUrl: '',
      createdAt: '2026-08-12T14:20:00Z',
      comments: []
    },
    {
      id: 3,
      placeId: 3,
      userId: 103,
      userName: 'Lê Hoàng Nam',
      userAvatar: 'https://images.unsplash.com/photo-1507003211169-0a1dd7228f2d?w=150',
      rating: 5,
      content: 'Cầu Vàng nhìn thực tế bên ngoài rất hùng vĩ và ngoạn mục. Làng Pháp mờ sương tạo cảm giác như đang ở châu Âu.',
      images: ['https://images.unsplash.com/photo-1559592413-7cec4d0cae2b?w=500'],
      videoUrl: '',
      createdAt: '2026-08-14T16:00:00Z',
      comments: []
    }
  ];

  // User Profile
  const defaultUser = {
    id: 101,
    fullName: 'Nguyễn Văn An',
    email: 'an.nguyen@gmail.com',
    phone: '0912 345 678',
    avatar: 'https://images.unsplash.com/photo-1535713875002-d1d0cf377fde?w=150',
    joinedDate: 'Tháng 3, 2025',
    visitedPrivacy: 'public'
  };

  // Helper Initializer
  function initData() {
    if (!localStorage.getItem(STORAGE_KEYS.PLACES)) {
      localStorage.setItem(STORAGE_KEYS.PLACES, JSON.stringify(defaultPlaces));
    }
    if (!localStorage.getItem(STORAGE_KEYS.FOODS)) {
      localStorage.setItem(STORAGE_KEYS.FOODS, JSON.stringify(defaultFoods));
    }
    if (!localStorage.getItem(STORAGE_KEYS.REVIEWS)) {
      localStorage.setItem(STORAGE_KEYS.REVIEWS, JSON.stringify(defaultReviews));
    }
    if (!localStorage.getItem(STORAGE_KEYS.FAVORITES)) {
      localStorage.setItem(STORAGE_KEYS.FAVORITES, JSON.stringify([2, 3]));
    }
    if (!localStorage.getItem(STORAGE_KEYS.VISIT_LOGS)) {
      localStorage.setItem(STORAGE_KEYS.VISIT_LOGS, JSON.stringify([
        { id: 1, placeId: 1, visitedDate: '2026-08-05', privacy: 'public' },
        { id: 2, placeId: 2, visitedDate: '2026-07-28', privacy: 'private' }
      ]));
    }
    if (!localStorage.getItem(STORAGE_KEYS.HISTORY)) {
      localStorage.setItem(STORAGE_KEYS.HISTORY, JSON.stringify([
        { placeId: 1, viewedAt: '2026-08-18T10:00:00Z' },
        { placeId: 3, viewedAt: '2026-08-18T08:30:00Z' }
      ]));
    }
    if (!localStorage.getItem(STORAGE_KEYS.NOTIFICATIONS)) {
      localStorage.setItem(STORAGE_KEYS.NOTIFICATIONS, JSON.stringify([
        { id: 1, title: 'Đề xuất được duyệt', content: 'Địa điểm "Cà phê Giảng" bạn đóng góp đã được duyệt!', isRead: false, time: '2 giờ trước', type: 'approved' },
        { id: 2, title: 'Phản hồi bình luận', content: 'Trần Thị Mai vừa trả lời bài đánh giá của bạn.', isRead: true, time: '1 ngày trước', type: 'comment' }
      ]));
    }
  }

  // Public Methods
  return {
    init: initData,
    getRegions: () => regions,
    getProvinces: () => provinces,
    getPlaceTypes: () => placeTypes,
    getCategories: () => categories,

    getPlaces: () => {
      initData();
      return JSON.parse(localStorage.getItem(STORAGE_KEYS.PLACES));
    },

    getPlaceById: (id) => {
      const places = TravelData.getPlaces();
      return places.find(p => p.id === parseInt(id));
    },

    savePlace: (newPlace) => {
      const places = TravelData.getPlaces();
      newPlace.id = Date.now();
      newPlace.avgRating = 0;
      newPlace.reviewCount = 0;
      places.unshift(newPlace);
      localStorage.setItem(STORAGE_KEYS.PLACES, JSON.stringify(places));
      return newPlace;
    },

    getFoods: () => {
      initData();
      return JSON.parse(localStorage.getItem(STORAGE_KEYS.FOODS));
    },

    getReviewsByPlaceId: (placeId) => {
      initData();
      const allReviews = JSON.parse(localStorage.getItem(STORAGE_KEYS.REVIEWS));
      return allReviews.filter(r => r.placeId === parseInt(placeId));
    },

    addReview: (reviewData) => {
      const allReviews = JSON.parse(localStorage.getItem(STORAGE_KEYS.REVIEWS));
      const user = TravelData.getUser();
      const newReview = {
        id: Date.now(),
        placeId: parseInt(reviewData.placeId),
        userId: user ? user.id : 101,
        userName: user ? user.fullName : 'Khách',
        userAvatar: user ? user.avatar : '',
        rating: parseInt(reviewData.rating),
        content: reviewData.content,
        images: reviewData.images || [],
        videoUrl: reviewData.videoUrl || '',
        createdAt: new Date().toISOString(),
        comments: []
      };

      allReviews.unshift(newReview);
      localStorage.setItem(STORAGE_KEYS.REVIEWS, JSON.stringify(allReviews));

      // Recalculate place rating
      TravelData.recalculatePlaceRating(reviewData.placeId);
      return newReview;
    },

    recalculatePlaceRating: (placeId) => {
      const places = TravelData.getPlaces();
      const reviews = TravelData.getReviewsByPlaceId(placeId);
      const place = places.find(p => p.id === parseInt(placeId));
      if (place) {
        place.reviewCount = reviews.length;
        if (reviews.length > 0) {
          const sum = reviews.reduce((acc, r) => acc + r.rating, 0);
          place.avgRating = parseFloat((sum / reviews.length).toFixed(1));
        } else {
          place.avgRating = 0;
        }
        localStorage.setItem(STORAGE_KEYS.PLACES, JSON.stringify(places));
      }
    },

    addCommentToReview: (reviewId, content) => {
      const allReviews = JSON.parse(localStorage.getItem(STORAGE_KEYS.REVIEWS));
      const user = TravelData.getUser();
      const review = allReviews.find(r => r.id === parseInt(reviewId));
      if (review) {
        if (!review.comments) review.comments = [];
        const newComment = {
          id: Date.now(),
          userName: user ? user.fullName : 'Khách',
          content: content,
          createdAt: new Date().toISOString()
        };
        review.comments.push(newComment);
        localStorage.setItem(STORAGE_KEYS.REVIEWS, JSON.stringify(allReviews));
        return newComment;
      }
      return null;
    },

    getFavorites: () => {
      initData();
      return JSON.parse(localStorage.getItem(STORAGE_KEYS.FAVORITES)) || [];
    },

    toggleFavorite: (placeId) => {
      const favs = TravelData.getFavorites();
      const id = parseInt(placeId);
      const idx = favs.indexOf(id);
      let isFav = false;
      if (idx > -1) {
        favs.splice(idx, 1);
        isFav = false;
      } else {
        favs.push(id);
        isFav = true;
      }
      localStorage.setItem(STORAGE_KEYS.FAVORITES, JSON.stringify(favs));
      return isFav;
    },

    isFavorite: (placeId) => {
      const favs = TravelData.getFavorites();
      return favs.includes(parseInt(placeId));
    },

    getVisitLogs: () => {
      initData();
      return JSON.parse(localStorage.getItem(STORAGE_KEYS.VISIT_LOGS)) || [];
    },

    addVisitLog: (placeId, visitedDate, privacy = 'public') => {
      const logs = TravelData.getVisitLogs();
      const newLog = {
        id: Date.now(),
        placeId: parseInt(placeId),
        visitedDate: visitedDate,
        privacy: privacy
      };
      logs.unshift(newLog);
      localStorage.setItem(STORAGE_KEYS.VISIT_LOGS, JSON.stringify(logs));
      return newLog;
    },

    removeVisitLog: (logId) => {
      const logs = TravelData.getVisitLogs().filter(l => l.id !== parseInt(logId));
      localStorage.setItem(STORAGE_KEYS.VISIT_LOGS, JSON.stringify(logs));
    },

    getHistory: () => {
      initData();
      return JSON.parse(localStorage.getItem(STORAGE_KEYS.HISTORY)) || [];
    },

    addToHistory: (placeId) => {
      let hist = TravelData.getHistory();
      const id = parseInt(placeId);
      hist = hist.filter(h => h.placeId !== id);
      hist.unshift({ placeId: id, viewedAt: new Date().toISOString() });
      if (hist.length > 20) hist.pop();
      localStorage.setItem(STORAGE_KEYS.HISTORY, JSON.stringify(hist));
    },

    clearHistory: () => {
      localStorage.setItem(STORAGE_KEYS.HISTORY, JSON.stringify([]));
    },

    getNotifications: () => {
      initData();
      return JSON.parse(localStorage.getItem(STORAGE_KEYS.NOTIFICATIONS)) || [];
    },

    addNotification: (noti) => {
      const notis = TravelData.getNotifications();
      const newNoti = {
        id: Date.now(),
        title: noti.title || 'Thông báo mới',
        content: noti.content || '',
        isRead: false,
        time: noti.time || 'Vừa xong',
        type: noti.type || 'system',
        targetUrl: noti.targetUrl || ''
      };
      notis.unshift(newNoti);
      localStorage.setItem(STORAGE_KEYS.NOTIFICATIONS, JSON.stringify(notis));
      return newNoti;
    },

    markNotificationRead: (id) => {
      const notis = TravelData.getNotifications();
      const n = notis.find(item => item.id === parseInt(id));
      if (n) n.isRead = true;
      localStorage.setItem(STORAGE_KEYS.NOTIFICATIONS, JSON.stringify(notis));
    },

    markAllNotificationsRead: () => {
      const notis = TravelData.getNotifications();
      notis.forEach(n => n.isRead = true);
      localStorage.setItem(STORAGE_KEYS.NOTIFICATIONS, JSON.stringify(notis));
    },

    deleteNotification: (id) => {
      let notis = TravelData.getNotifications();
      notis = notis.filter(item => item.id !== parseInt(id));
      localStorage.setItem(STORAGE_KEYS.NOTIFICATIONS, JSON.stringify(notis));
    },

    clearNotifications: () => {
      localStorage.setItem(STORAGE_KEYS.NOTIFICATIONS, JSON.stringify([]));
    },

    getUser: () => {
      const u = localStorage.getItem(STORAGE_KEYS.USER);
      return u ? JSON.parse(u) : null;
    },

    getUserProfile: () => {
      return TravelData.getUser();
    },

    updateUser: (userData) => {
      const current = TravelData.getUser() || {};
      const user = { ...current, ...userData };
      localStorage.setItem(STORAGE_KEYS.USER, JSON.stringify(user));
      if (typeof api !== 'undefined') {
        api.setCurrentUser(user);
      }
      return user;
    },

    logoutUser: () => {
      localStorage.removeItem(STORAGE_KEYS.USER);
      localStorage.removeItem('currentUser');
      localStorage.removeItem('tr_user_profile');
      if (typeof api !== 'undefined') {
        api.setCurrentUser(null);
      }
    }
  };
})();

// Auto initialize data immediately
TravelData.init();
