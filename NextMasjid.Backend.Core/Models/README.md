﻿/*
 *  List of Services
 *      GetMosquesByBound       [input sw and ne points]    [result: arrayOf MosqueSimple[string nameEn, string nameAr, string latlng]]
 *      GetScoresByBound        [input sw and ne points]    [result: arrayOf ScoreSimple[string latlng, int value]]
 *      GetProvinceCity         [input: none]               [result: arrayOf ProvinceSimple[string provinceNameAr provinceNameEn, arrayOf CitySimple [int cityID, cityNameAr, cityNameEn, string centreLatLng]]]
 *      GetEditorChoice         [input: none]               [result: arrayOf EditorChoiceSimple [string latlng]]
 *      GetLocationScore        [input: string latlng]      [result: ScoreSimple[string latlng, int value]]
 *      GetLocationScoreDetails [input: string latlng]      [result: ScoreDetailsSimple [string lattlng, int value, int 1, int 2, int 3, int 4, arrayOf MosqueDistanceSimple[string nameEn, string nameAr, string latlng, string distanceEn, string distanceAr]]
 *      
 *      
 *      
 *   Masjids        // 1
 *   Cities         // 1
 *   Scores         // 3
 *   EditorsChoice  // 1
 *   
 */