//
//  MAUnityPlugin.mm
//  AppLovin MAX Unity Plugin
//

#pragma clang diagnostic push
#pragma clang diagnostic ignored "-Wdeprecated-declarations"

#import "MAUnityAdManager.h"

#define NSSTRING(_X) ( (_X != NULL) ? [NSString stringWithCString: _X encoding: NSStringEncodingConversionAllowLossy].al_stringByTrimmingWhitespace : nil)

@interface NSString (ALUtils)
@property (nonatomic, copy, readonly) NSString *al_stringByTrimmingWhitespace;
@property (assign, readonly, getter=al_isValidString) BOOL al_validString;
@end

UIView* UnityGetGLView();

// When native code plugin is implemented in .mm / .cpp file, then functions
// should be surrounded with extern "C" block to conform C function naming rules
extern "C"
{
    static NSString *const TAG = @"MAUnityPlugin";
    
    static ALSdk *_sdk;
    static MAUnityAdManager *_adManager;
    static bool _isPluginInitialized = false;
    static bool _isSdkInitialized = false;
    static ALSdkConfiguration *_sdkConfiguration;
    
    // Store these values if pub attempts to set it before calling _MaxInitializeSdk()
    static NSString *_userIdentifierToSet;
    static NSString *_userSegmentNameToSet;
    static NSArray<NSString *> *_testDeviceIdentifiersToSet;
    static NSNumber *_verboseLoggingToSet;
    static NSNumber *_creativeDebuggerEnabledToSet;
    static NSNumber *_exceptionHandlerEnabledToSet;
    static NSNumber *_locationCollectionEnabledToSet;
    static NSMutableDictionary<NSString *, NSString *> *_extraParametersToSet = [NSMutableDictionary dictionary];
    static NSObject *_extraParametersToSetLock = [[NSObject alloc] init];
    
    // Helper method to create C string copy
    static const char * cStringCopy(NSString *string);
    
    bool isPluginInitialized()
    {
        return _isPluginInitialized;
    }
    
    void maybeInitializePlugin()
    {
        if ( isPluginInitialized() ) return;
        
        _adManager = [MAUnityAdManager shared];
        _isPluginInitialized = true;
    }

    NSArray<NSString *> * toStringArray(char **arrayPointer, int size)
    {
        NSMutableArray<NSString *> *array = [NSMutableArray arrayWithCapacity: size];
        for ( int i = 0; i < size; i++ )
        {
            [array addObject: NSSTRING(arrayPointer[i])];
        }
        
        return array;
    }
    
    void setPendingExtraParametersIfNeeded(ALSdkSettings *settings)
    {
        NSDictionary *extraParameters;
        @synchronized ( _extraParametersToSetLock )
        {
            if ( _extraParametersToSet.count <= 0 ) return;
            
            extraParameters = [NSDictionary dictionaryWithDictionary: _extraParametersToSet];
            [_extraParametersToSet removeAllObjects];
        }
        
        for ( NSString *key in extraParameters.allKeys )
        {
            [settings setExtraParameterForKey: key value: extraParameters[key]];
        }
    }
    
    
    void _MaxSetSdkKey(const char *sdkKey)
    {
        maybeInitializePlugin();
        
        if (!sdkKey) return;
        
        NSString *sdkKeyStr = [NSString stringWithUTF8String: sdkKey];
        
        NSDictionary *infoDict = [[NSBundle mainBundle] infoDictionary];
        [infoDict setValue: sdkKeyStr forKey: @"AppLovinSdkKey"];
    }
    
    void _MaxInitializeSdk(const char *serializedAdUnitIdentifiers, const char *serializedMetaData, ALUnityBackgroundCallback backgroundCallback)
    {
        maybeInitializePlugin();
        
        _sdk = [_adManager initializeSdkWithAdUnitIdentifiers: NSSTRING(serializedAdUnitIdentifiers)
                                                     metaData: NSSTRING(serializedMetaData)
                                           backgroundCallback: backgroundCallback
                                         andCompletionHandler:^(ALSdkConfiguration *configuration) {
            _sdkConfiguration = configuration;
            _isSdkInitialized = true;
        }];
        
        if ( _userIdentifierToSet )
        {
            _sdk.userIdentifier = _userIdentifierToSet;
            _userIdentifierToSet = nil;
        }
        
        if ( _userSegmentNameToSet )
        {
            _sdk.userSegment.name = _userSegmentNameToSet;
            _userSegmentNameToSet = nil;
        }
        
        if ( _testDeviceIdentifiersToSet )
        {
            _sdk.settings.testDeviceAdvertisingIdentifiers = _testDeviceIdentifiersToSet;
            _testDeviceIdentifiersToSet = nil;
        }
        
        if ( _verboseLoggingToSet )
        {
            _sdk.settings.isVerboseLogging = _verboseLoggingToSet.boolValue;
            _verboseLoggingToSet = nil;
        }

        if ( _creativeDebuggerEnabledToSet )
        {
            _sdk.settings.creativeDebuggerEnabled = _creativeDebuggerEnabledToSet.boolValue;
            _creativeDebuggerEnabledToSet = nil;
        }

        if ( _exceptionHandlerEnabledToSet )
        {
            _sdk.settings.exceptionHandlerEnabled = _exceptionHandlerEnabledToSet.boolValue;
            _exceptionHandlerEnabledToSet = nil;
        }
        
        if ( _locationCollectionEnabledToSet )
        {
            _sdk.settings.locationCollectionEnabled = _locationCollectionEnabledToSet.boolValue;
            _locationCollectionEnabledToSet = nil;
        }
        
        setPendingExtraParametersIfNeeded( _sdk.settings );
    }
    
    bool _MaxIsInitialized()
    {
        return _isPluginInitialized && _isSdkInitialized;
    }

    const char * _MaxGetAvailableMediatedNetworks()
    {
        if ( !_sdk )
        {
            NSLog(@"[%@] Failed to get available mediated networks - please ensure the AppLovin MAX Unity Plugin has been initialized by calling 'MaxSdk.InitializeSdk();'!", TAG);
            return cStringCopy(@"");
        }
        
        NSArray<MAMediatedNetworkInfo *> *availableMediatedNetworks = [_sdk availableMediatedNetworks];
        
        // Create array of serialized network strings
        NSMutableArray<NSDictionary<NSString *, NSString *> *> *serializedNetworks = [NSMutableArray arrayWithCapacity: availableMediatedNetworks.count];
        for ( MAMediatedNetworkInfo *mediatedNetwork in availableMediatedNetworks )
        {
            NSDictionary<NSString *, NSString *> *mediatedNetworkDictionary = @{@"name" : mediatedNetwork.name,
                                                                               @"adapterClassName" : mediatedNetwork.adapterClassName,
                                                                               @"adapterVersion" : mediatedNetwork.adapterVersion,
                                                                               @"sdkVersion" : mediatedNetwork.sdkVersion};
            [serializedNetworks addObject: mediatedNetworkDictionary];
        }
        
        NSData *jsonData = [NSJSONSerialization dataWithJSONObject: serializedNetworks options: 0 error: nil];
        return cStringCopy([[NSString alloc] initWithData: jsonData encoding: NSUTF8StringEncoding]);
    }
    
    void _MaxShowMediationDebugger()
    {
        if ( !_sdk )
        {
            NSLog(@"[%@] Failed to show mediation debugger - please ensure the AppLovin MAX Unity Plugin has been initialized by calling 'MaxSdk.InitializeSdk();'!", TAG);
            return;
        }
        
        [_sdk showMediationDebugger];
    }

    void _MaxShowConsentDialog()
    {
        NSLog(@"[%@] Failed to show consent dialog - Unavailable on iOS, please use the consent flow: https://dash.applovin.com/documentation/mediation/unity/getting-started/consent-flow", TAG);
    }
    
    int _MaxConsentDialogState()
    {
        if (!isPluginInitialized()) return ALConsentDialogStateUnknown;
        
        return (int) _sdkConfiguration.consentDialogState;
    }
    
    void _MaxSetUserId(const char *userId)
    {
        if ( _sdk )
        {
            _sdk.userIdentifier = NSSTRING(userId);
        }
        else
        {
            _userIdentifierToSet = NSSTRING(userId);
        }
    }
    
    void _MaxSetUserSegmentField(const char *serializedKey, const char *serializedValue)
    {
        // NSString *key = NSSTRING(serializedKey); // To be ignored until we add more properties
        NSString *value = NSSTRING(serializedValue);
        
        if ( _sdk )
        {
            _sdk.userSegment.name = value;
        }
        else
        {
            _userSegmentNameToSet = value;
        }
    }

    void _MaxSetTargetingDataYearOfBirth(const int yearOfBirth)
    {
        if ( !isPluginInitialized() ) return;
        _sdk.targetingData.yearOfBirth = yearOfBirth <= 0 ? nil : @(yearOfBirth);
    }

    void _MaxSetTargetingDataGender(const char *gender)
    {
        if ( !isPluginInitialized() ) return;
        
        NSString *genderString = NSSTRING(gender);
        ALGender alGender = ALGenderUnknown;
        
        if ( [genderString isEqualToString: @"F"] )
        {
            alGender = ALGenderFemale;
        }
        else if ( [genderString isEqualToString: @"M"] )
        {
            alGender = ALGenderMale;
        }
        else if ( [genderString isEqualToString: @"O"] )
        {
            alGender = ALGenderOther;
        }
        
        _sdk.targetingData.gender = alGender;
    }

    void _MaxSetTargetingDataMaximumAdContentRating(const int maximumAdContentRating)
    {
        if ( !isPluginInitialized() ) return;
        
        ALAdContentRating rating = ALAdContentRatingNone;
        
        if ( maximumAdContentRating == 1 )
        {
            rating = ALAdContentRatingAllAudiences;
        }
        else if ( maximumAdContentRating == 2 )
        {
            rating = ALAdContentRatingEveryoneOverTwelve;
        }
        else if ( maximumAdContentRating == 3 )
        {
            rating = ALAdContentRatingMatureAudiences;
        }
        
        _sdk.targetingData.maximumAdContentRating = rating;
    }

    void _MaxSetTargetingDataEmail(const char *email)
    {
        if ( !isPluginInitialized() ) return;
        _sdk.targetingData.email = NSSTRING(email);
    }

    void _MaxSetTargetingDataPhoneNumber(const char *phoneNumber)
    {
        if ( !isPluginInitialized() ) return;
        _sdk.targetingData.phoneNumber = NSSTRING(phoneNumber);
    }

    void _MaxSetTargetingDataKeywords(char **keywords, int size)
    {
        if ( !isPluginInitialized() ) return;
        _sdk.targetingData.keywords = toStringArray(keywords, size);
    }

    void _MaxSetTargetingDataInterests(char **interests, int size)
    {
        if ( !isPluginInitialized() ) return;
        _sdk.targetingData.interests = toStringArray(interests, size);
    }

    void _MaxClearAllTargetingData()
    {
        if ( !isPluginInitialized() ) return;
        [_sdk.targetingData clearAll];
    }

    const char * _MaxGetSdkConfiguration()
    {
        if ( !_sdk )
        {
            NSLog(@"[%@] Failed to get SDK configuration - please ensure the AppLovin MAX Unity Plugin has been initialized by calling 'MaxSdk.InitializeSdk();'!", TAG);
            return cStringCopy(@"");
        }
        
        NSString *consentDialogStateStr = @(_sdk.configuration.consentDialogState).stringValue;
        NSString *appTrackingStatus = @(_sdk.configuration.appTrackingTransparencyStatus).stringValue; // Deliberately name it `appTrackingStatus` to be a bit more generic (in case Android introduces a similar concept)

        return cStringCopy([MAUnityAdManager serializeParameters: @{@"consentDialogState" : consentDialogStateStr,
                                                                    @"countryCode" : _sdk.configuration.countryCode,
                                                                    @"appTrackingStatus" : appTrackingStatus,
                                                                    @"isSuccessfullyInitialized" : ([_sdk isInitialized] ? @"true" : @"false")}]);
    }
    
    void _MaxSetHasUserConsent(bool hasUserConsent)
    {
        [ALPrivacySettings setHasUserConsent: hasUserConsent];
    }
    
    bool _MaxHasUserConsent()
    {
        return [ALPrivacySettings hasUserConsent];
    }
    
    bool _MaxIsUserConsentSet()
    {
        return [ALPrivacySettings isUserConsentSet];
    }

    void _MaxSetIsAgeRestrictedUser(bool isAgeRestrictedUser)
    {
        [ALPrivacySettings setIsAgeRestrictedUser: isAgeRestrictedUser];
    }
    
    bool _MaxIsAgeRestrictedUser()
    {
        return [ALPrivacySettings isAgeRestrictedUser];
    }

    bool _MaxIsAgeRestrictedUserSet()
    {
        return [ALPrivacySettings isAgeRestrictedUserSet];
    }
    
    void _MaxSetDoNotSell(bool doNotSell)
    {
        [ALPrivacySettings setDoNotSell: doNotSell];
    }
    
    bool _MaxIsDoNotSell()
    {
        return [ALPrivacySettings isDoNotSell];
    }

    bool _MaxIsDoNotSellSet()
    {
        return [ALPrivacySettings isDoNotSellSet];
    }
    
    void _MaxCreateBanner(const char *adUnitIdentifier, const char *bannerPosition)
    {
        if (!isPluginInitialized()) return;
        
        [_adManager createBannerWithAdUnitIdentifier: NSSTRING(adUnitIdentifier) atPosition: NSSTRING(bannerPosition)];
    }

    void _MaxCreateBannerXY(const char *adUnitIdentifier, const float x, const float y)
    {
        if (!isPluginInitialized()) return;
        
        [_adManager createBannerWithAdUnitIdentifier: NSSTRING(adUnitIdentifier) x: x y: y];
    }
    
    void _MaxSetBannerBackgroundColor(const char *adUnitIdentifier, const char *hexColorCode)
    {
        if (!isPluginInitialized()) return;
        
        [_adManager setBannerBackgroundColorForAdUnitIdentifier: NSSTRING(adUnitIdentifier) hexColorCode: NSSTRING(hexColorCode)];
    }
    
    void _MaxSetBannerPlacement(const char *adUnitIdentifier, const char *placement)
    {
        [_adManager setBannerPlacement: NSSTRING(placement) forAdUnitIdentifier: NSSTRING(adUnitIdentifier)];
    }
    
    void _MaxStartBannerAutoRefresh(const char *adUnitIdentifier)
    {
        if (!isPluginInitialized()) return;
        
        [_adManager startBannerAutoRefreshForAdUnitIdentifier: NSSTRING(adUnitIdentifier)];
    }
    
    void _MaxStopBannerAutoRefresh(const char *adUnitIdentifier)
    {
        if (!isPluginInitialized()) return;
        
        [_adManager stopBannerAutoRefreshForAdUnitIdentifier: NSSTRING(adUnitIdentifier)];
    }
    
    void _MaxSetBannerExtraParameter(const char *adUnitIdentifier, const char *key, const char *value)
    {
        [_adManager setBannerExtraParameterForAdUnitIdentifier: NSSTRING(adUnitIdentifier)
                                                           key: NSSTRING(key)
                                                         value: NSSTRING(value)];
    }
    
    void _MaxSetBannerLocalExtraParameter(const char *adUnitIdentifier, const char *key, MAUnityRef value)
    {
        [_adManager setBannerLocalExtraParameterForAdUnitIdentifier: NSSTRING(adUnitIdentifier)
                                                                key: NSSTRING(key)
                                                              value: (__bridge id) value];
    }
    
    void _MaxSetBannerCustomData(const char *adUnitIdentifier, const char *customData)
    {
        [_adManager setBannerCustomData: NSSTRING(customData) forAdUnitIdentifier: NSSTRING(adUnitIdentifier)];
    }

    void _MaxSetBannerWidth(const char *adUnitIdentifier, const float width)
    {
        [_adManager setBannerWidth: width forAdUnitIdentifier: NSSTRING(adUnitIdentifier)];
    }
    
    void _MaxUpdateBannerPosition(const char *adUnitIdentifier, const char *bannerPosition)
    {
        [_adManager updateBannerPosition: NSSTRING(bannerPosition) forAdUnitIdentifier: NSSTRING(adUnitIdentifier)];
    }

    void _MaxUpdateBannerPositionXY(const char *adUnitIdentifier, const float x, const float y)
    {
        [_adManager updateBannerPosition: x y: y forAdUnitIdentifier: NSSTRING(adUnitIdentifier)];
    }
    
    void _MaxShowBanner(const char *adUnitIdentifier)
    {
        if (!isPluginInitialized()) return;
        
        [_adManager showBannerWithAdUnitIdentifier: NSSTRING(adUnitIdentifier)];
    }
    
    void _MaxDestroyBanner(const char *adUnitIdentifier)
    {
        if (!isPluginInitialized()) return;
        
        [_adManager destroyBannerWithAdUnitIdentifier: NSSTRING(adUnitIdentifier)];
    }
    
    void _MaxHideBanner(const char *adUnitIdentifier)
    {
        if (!isPluginInitialized()) return;
        
        [_adManager hideBannerWithAdUnitIdentifier: NSSTRING(adUnitIdentifier)];
    }
    
    const char * _MaxGetBannerLayout(const char *adUnitIdentifier)
    {
        if (!isPluginInitialized()) return cStringCopy(@"");
        
        return cStringCopy([_adManager bannerLayoutForAdUnitIdentifier: NSSTRING(adUnitIdentifier)]);
    }
    
    void _MaxCreateMRec(const char *adUnitIdentifier, const char *mrecPosition)
    {
        if (!isPluginInitialized()) return;
        
        [_adManager createMRecWithAdUnitIdentifier: NSSTRING(adUnitIdentifier) atPosition: NSSTRING(mrecPosition)];
    }
    
    void _MaxCreateMRecXY(const char *adUnitIdentifier, const float x, const float y)
    {
        if (!isPluginInitialized()) return;
        
        [_adManager createMRecWithAdUnitIdentifier: NSSTRING(adUnitIdentifier) x: x y: y];
    }
    
    void _MaxSetMRecPlacement(const char *adUnitIdentifier, const char *placement)
    {
        [_adManager setMRecPlacement: NSSTRING(placement) forAdUnitIdentifier: NSSTRING(adUnitIdentifier)];
    }
    
    void _MaxStartMRecAutoRefresh(const char *adUnitIdentifier)
    {
        if (!isPluginInitialized()) return;
        
        [_adManager startMRecAutoRefreshForAdUnitIdentifier: NSSTRING(adUnitIdentifier)];
    }
    
    void _MaxStopMRecAutoRefresh(const char *adUnitIdentifier)
    {
        if (!isPluginInitialized()) return;
        
        [_adManager stopMRecAutoRefreshForAdUnitIdentifier: NSSTRING(adUnitIdentifier)];
    }
    
    void _MaxUpdateMRecPosition(const char *adUnitIdentifier, const char *mrecPosition)
    {
        [_adManager updateMRecPosition: NSSTRING(mrecPosition) forAdUnitIdentifier: NSSTRING(adUnitIdentifier)];
    }
    
    void _MaxUpdateMRecPositionXY(const char *adUnitIdentifier, const float x, const float y)
    {
        [_adManager updateMRecPosition: x y: y forAdUnitIdentifier: NSSTRING(adUnitIdentifier)];
    }
    
    void _MaxShowMRec(const char *adUnitIdentifier)
    {
        if (!isPluginInitialized()) return;
        
        [_adManager showMRecWithAdUnitIdentifier: NSSTRING(adUnitIdentifier)];
    }
    
    void _MaxDestroyMRec(const char *adUnitIdentifier)
    {
        if (!isPluginInitialized()) return;
        
        [_adManager destroyMRecWithAdUnitIdentifier: NSSTRING(adUnitIdentifier)];
    }
    
    void _MaxHideMRec(const char *adUnitIdentifier)
    {
        if (!isPluginInitialized()) return;
        
        [_adManager hideMRecWithAdUnitIdentifier: NSSTRING(adUnitIdentifier)];
    }

    void _MaxSetMRecExtraParameter(const char *adUnitIdentifier, const char *key, const char *value)
    {
        [_adManager setMRecExtraParameterForAdUnitIdentifier: NSSTRING(adUnitIdentifier)
                                                         key: NSSTRING(key)
                                                       value: NSSTRING(value)];
    }
    
    void _MaxSetMRecLocalExtraParameter(const char *adUnitIdentifier, const char *key, MAUnityRef value)
    {
        [_adManager setMRecLocalExtraParameterForAdUnitIdentifier: NSSTRING(adUnitIdentifier)
                                                              key: NSSTRING(key)
                                                            value: (__bridge id)value];
    }
    
    void _MaxSetMRecCustomData(const char *adUnitIdentifier, const char *customData)
    {
        [_adManager setMRecCustomData: NSSTRING(customData) forAdUnitIdentifier: NSSTRING(adUnitIdentifier)];
    }

    const char * _MaxGetMRecLayout(const char *adUnitIdentifier)
    {
        if (!isPluginInitialized()) return cStringCopy(@"");
        
        return cStringCopy([_adManager mrecLayoutForAdUnitIdentifier: NSSTRING(adUnitIdentifier)]);
    }

    void _MaxCreateCrossPromoAd(const char *adUnitIdentifier, const float x, const float y, const float width, const float height, const float rotation)
    {
        if (!isPluginInitialized()) return;
        
        [_adManager createCrossPromoAdWithAdUnitIdentifier: NSSTRING(adUnitIdentifier) x: x y: y width: width height: height rotation: rotation];
    }

    void _MaxSetCrossPromoAdPlacement(const char *adUnitIdentifier, const char *placement)
    {
        [_adManager setCrossPromoAdPlacement: NSSTRING(placement) forAdUnitIdentifier: NSSTRING(adUnitIdentifier)];
    }

    void _MaxUpdateCrossPromoAdPosition(const char *adUnitIdentifier, const float x, const float y, const float width, const float height, const float rotation)
    {
        [_adManager updateCrossPromoAdPositionForAdUnitIdentifier: NSSTRING(adUnitIdentifier) x: x y: y width: width height: height rotation: rotation];
    }

    void _MaxShowCrossPromoAd(const char *adUnitIdentifier)
    {
        if (!isPluginInitialized()) return;
        
        [_adManager showCrossPromoAdWithAdUnitIdentifier: NSSTRING(adUnitIdentifier)];
    }

    void _MaxDestroyCrossPromoAd(const char *adUnitIdentifier)
    {
        if (!isPluginInitialized()) return;
        
        [_adManager destroyCrossPromoAdWithAdUnitIdentifier: NSSTRING(adUnitIdentifier)];
    }

    void _MaxHideCrossPromoAd(const char *adUnitIdentifier)
    {
        if (!isPluginInitialized()) return;
        
        [_adManager hideCrossPromoAdWithAdUnitIdentifier: NSSTRING(adUnitIdentifier)];
    }

    const char * _MaxGetCrossPromoAdLayout(const char *adUnitIdentifier)
    {
        if (!isPluginInitialized()) return cStringCopy(@"");
        
        return cStringCopy([_adManager crossPromoAdLayoutForAdUnitIdentifier: NSSTRING(adUnitIdentifier)]);
    }
    
    void _MaxLoadInterstitial(const char *adUnitIdentifier)
    {
        if (!isPluginInitialized()) return;
        
        [_adManager loadInterstitialWithAdUnitIdentifier: NSSTRING(adUnitIdentifier)];
    }
    
    void _MaxSetInterstitialExtraParameter(const char *adUnitIdentifier, const char *key, const char *value)
    {
        [_adManager setInterstitialExtraParameterForAdUnitIdentifier: NSSTRING(adUnitIdentifier)
                                                                 key: NSSTRING(key)
                                                               value: NSSTRING(value)];
    }
    
    void _MaxSetInterstitialLocalExtraParameter(const char *adUnitIdentifier, const char *key, MAUnityRef value)
    {
        [_adManager setInterstitialLocalExtraParameterForAdUnitIdentifier: NSSTRING(adUnitIdentifier)
                                                                      key: NSSTRING(key)
                                                                    value: (__bridge id)value];
    }

    bool _MaxIsInterstitialReady(const char *adUnitIdentifier)
    {
        if (!isPluginInitialized()) return false;
        
        return [_adManager isInterstitialReadyWithAdUnitIdentifier: NSSTRING(adUnitIdentifier)];
    }
    
    void _MaxShowInterstitial(const char *adUnitIdentifier, const char *placement, const char *customData)
    {
        if (!isPluginInitialized()) return;
        
        [_adManager showInterstitialWithAdUnitIdentifier: NSSTRING(adUnitIdentifier) placement: NSSTRING(placement) customData: NSSTRING(customData)];
    }
    
    void _MaxLoadRewardedAd(const char *adUnitIdentifier)
    {
        if (!isPluginInitialized()) return;
        
        [_adManager loadRewardedAdWithAdUnitIdentifier: NSSTRING(adUnitIdentifier)];
    }
    
    void _MaxSetRewardedAdExtraParameter(const char *adUnitIdentifier, const char *key, const char *value)
    {
        [_adManager setRewardedAdExtraParameterForAdUnitIdentifier: NSSTRING(adUnitIdentifier)
                                                               key: NSSTRING(key)
                                                             value: NSSTRING(value)];
    }
    
    void _MaxSetRewardedAdLocalExtraParameter(const char *adUnitIdentifier, const char *key, MAUnityRef value)
    {
        [_adManager setRewardedAdLocalExtraParameterForAdUnitIdentifier: NSSTRING(adUnitIdentifier)
                                                                    key: NSSTRING(key)
                                                                  value: (__bridge id)value];
    }

    bool _MaxIsRewardedAdReady(const char *adUnitIdentifier)
    {
        if (!isPluginInitialized()) return false;
        
        return [_adManager isRewardedAdReadyWithAdUnitIdentifier: NSSTRING(adUnitIdentifier)];
    }
    
    void _MaxShowRewardedAd(const char *adUnitIdentifier, const char *placement, const char *customData)
    {
        if (!isPluginInitialized()) return;
        
        [_adManager showRewardedAdWithAdUnitIdentifier: NSSTRING(adUnitIdentifier) placement: NSSTRING(placement) customData: NSSTRING(customData)];
    }
    
    void _MaxLoadRewardedInterstitialAd(const char *adUnitIdentifier)
    {
        if (!isPluginInitialized()) return;
        
        [_adManager loadRewardedInterstitialAdWithAdUnitIdentifier: NSSTRING(adUnitIdentifier)];
    }
    
    void _MaxSetRewardedInterstitialAdExtraParameter(const char *adUnitIdentifier, const char *key, const char *value)
    {
        [_adManager setRewardedInterstitialAdExtraParameterForAdUnitIdentifier: NSSTRING(adUnitIdentifier)
                                                                           key: NSSTRING(key)
                                                                         value: NSSTRING(value)];
    }
    
    void _MaxSetRewardedInterstitialAdLocalExtraParameter(const char *adUnitIdentifier, const char *key, MAUnityRef value)
    {
        [_adManager setRewardedInterstitialAdLocalExtraParameterForAdUnitIdentifier: NSSTRING(adUnitIdentifier)
                                                                                key: NSSTRING(key)
                                                                              value: (__bridge id)value];
    }

    bool _MaxIsRewardedInterstitialAdReady(const char *adUnitIdentifier)
    {
        if (!isPluginInitialized()) return false;
        
        return [_adManager isRewardedInterstitialAdReadyWithAdUnitIdentifier: NSSTRING(adUnitIdentifier)];
    }
    
    void _MaxShowRewardedInterstitialAd(const char *adUnitIdentifier, const char *placement, const char *customData)
    {
        if (!isPluginInitialized()) return;
        
        [_adManager showRewardedInterstitialAdWithAdUnitIdentifier: NSSTRING(adUnitIdentifier) placement: NSSTRING(placement) customData: NSSTRING(customData)];
    }
    
    void _MaxTrackEvent(const char *event, const char *parameters)
    {
        if (!isPluginInitialized()) return;
        
        [_adManager trackEvent: NSSTRING(event) parameters: NSSTRING(parameters)];
    }
        
    bool _MaxGetBool(const char *key, bool defaultValue)
    {
        if ( !_sdk ) return defaultValue;
        
        return [_sdk.variableService boolForKey: NSSTRING(key) defaultValue: defaultValue];
    }
    
    const char * _MaxGetString(const char *key, const char *defaultValue)
    {
        if ( !_sdk ) return defaultValue;
        
        return cStringCopy([_sdk.variableService stringForKey: NSSTRING(key) defaultValue: NSSTRING(defaultValue)]);
    }
    
    bool _MaxIsTablet()
    {
        return [UIDevice currentDevice].userInterfaceIdiom == UIUserInterfaceIdiomPad;
    }
    
    static const char * cStringCopy(NSString *string)
    {
        const char *value = string.UTF8String;
        return value ? strdup(value) : NULL;
    }
    
    void _MaxSetMuted(bool muted)
    {
        if ( !_sdk ) return;
        
        _sdk.settings.muted = muted;
    }
    
    bool _MaxIsMuted()
    {
        if ( !_sdk ) return false;
        
        return _sdk.settings.muted;
    }
    
    float _MaxScreenDensity()
    {
        return [UIScreen.mainScreen nativeScale];
    }
    
    const char * _MaxGetAdInfo(const char *adUnitIdentifier)
    {
        return cStringCopy([_adManager adInfoForAdUnitIdentifier: NSSTRING(adUnitIdentifier)]);
    }
    
    const char * _MaxGetAdValue(const char *adUnitIdentifier, const char *key)
    {
        return cStringCopy([_adManager adValueForAdUnitIdentifier: NSSTRING(adUnitIdentifier) withKey: NSSTRING(key)]);
    }

    void _MaxSetVerboseLogging(bool enabled)
    {
        if ( _sdk )
        {
            _sdk.settings.isVerboseLogging = enabled;
            _verboseLoggingToSet = nil;
        }
        else
        {
            _verboseLoggingToSet = @(enabled);
        }
    }
    
    bool _MaxIsVerboseLoggingEnabled()
    {
        if ( _sdk )
        {
            return _sdk.settings.isVerboseLogging;
        }
        else if ( _verboseLoggingToSet )
        {
            return _verboseLoggingToSet;
        }

        return false;
    }

    void _MaxSetTestDeviceAdvertisingIdentifiers(char **advertisingIdentifiers, int size)
    {
        NSArray<NSString *> *advertisingIdentifiersArray = toStringArray(advertisingIdentifiers, size);
        
        if ( _sdk )
        {
            _sdk.settings.testDeviceAdvertisingIdentifiers = advertisingIdentifiersArray;
            _testDeviceIdentifiersToSet = nil;
        }
        else
        {
            _testDeviceIdentifiersToSet = advertisingIdentifiersArray;
        }
    }

    void _MaxSetCreativeDebuggerEnabled(bool enabled)
    {
        if ( _sdk )
        {
            _sdk.settings.creativeDebuggerEnabled = enabled;
            _creativeDebuggerEnabledToSet = nil;
        }
        else
        {
            _creativeDebuggerEnabledToSet = @(enabled);
        }
    }
    
    void _MaxSetExceptionHandlerEnabled(bool enabled)
    {
        if ( _sdk )
        {
            _sdk.settings.exceptionHandlerEnabled = enabled;
            _exceptionHandlerEnabledToSet = nil;
        }
        else
        {
            _exceptionHandlerEnabledToSet = @(enabled);
        }
    }

    void _MaxSetLocationCollectionEnabled(bool enabled)
    {
        if ( _sdk )
        {
            _sdk.settings.locationCollectionEnabled = enabled;
            _locationCollectionEnabledToSet = nil;
        }
        else
        {
            _locationCollectionEnabledToSet = @(enabled);
        }
    }

    void _MaxSetExtraParameter(const char *key, const char *value)
    {
        NSString *stringKey = NSSTRING(key);
        if ( ![stringKey al_isValidString] )
        {
            NSLog(@"[%@] Failed to set extra parameter for nil or empty key: %@", TAG, stringKey);
            return;
        }
        
        if ( _sdk )
        {
            ALSdkSettings *settings = _sdk.settings;
            [settings setExtraParameterForKey: stringKey value: NSSTRING(value)];
            setPendingExtraParametersIfNeeded( settings );
        }
        else
        {
            @synchronized ( _extraParametersToSetLock )
            {
                _extraParametersToSet[stringKey] = NSSTRING(value);
            }
        }
    }

    float _MaxGetAdaptiveBannerHeight(const float width)
    {
        return [MAUnityAdManager adaptiveBannerHeightForWidth: width];
    }

    [[deprecated("This API has been deprecated. Please use our SDK's initialization callback to retrieve variables instead.")]]
    void _MaxLoadVariables()
    {
        if (!isPluginInitialized()) return;
        
        [_adManager loadVariables];
    }
}

#pragma clang diagnostic pop
