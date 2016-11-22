//
//  UserHookBinding.m
//  Unity-iPhone
//
//  Created by Matt Johnston on 11/10/16.
//
//


#import <Foundation/Foundation.h>
#import "UserHook/UserHook.h"

#import "UnityInterface.h"
#import "UnityAppController.h"

// Converts C style string to NSString
NSString* CreateNSString (const char* string)
{
    if (string)
        return [NSString stringWithUTF8String: string];
    else
        return [NSString stringWithUTF8String: ""];
}

const char * CreateConstChar(NSString * string) {
    return [string cStringUsingEncoding:NSASCIIStringEncoding];
}



@interface UserHookNotification : NSObject

+(UserHookNotification *) sharedInstance;

-(void) handlePushNotification:(NSNotification *) notification;

-(void) showNewFeedback;

@end


static UserHookNotification * _sharedUserHookNotification;

@implementation UserHookNotification

+(UserHookNotification *) sharedInstance {
    
    if (_sharedUserHookNotification == nil) {
        _sharedUserHookNotification = [[UserHookNotification alloc] init];
    }
    
    return  _sharedUserHookNotification;
}

-(void) handlePushNotification:(NSNotification *) notification {
    
    NSDictionary * userInfo = notification.userInfo;
    
    if ([UserHook isPushFromUserHook:userInfo]) {
        [UserHook handlePushNotification:userInfo];
    }
}

-(void) showNewFeedback {
    
    UnitySendMessage(CreateConstChar(@"UserHook"), CreateConstChar(@"showNewFeedback"), CreateConstChar(@""));
}
@end




#if __cplusplus
extern "C" {
#endif
    
    void _uhSetApplicationIdAndKey(const char * appId, const char * apiKey) {
        
        NSString * appIdString = CreateNSString(appId);
        NSString * appKeyString = CreateNSString(apiKey);
        [UserHook setApplicationId:appIdString apiKey:appKeyString];
        
        [UserHook setPayloadHandler:^(NSDictionary *payload) {
            
            if (!payload) {
            	payload = [NSDictionary new];
            }
            
            NSError *error;
            NSData *jsonData = [NSJSONSerialization dataWithJSONObject:payload
                                                               options:0
                                                                 error:&error];
            
            if (!jsonData) {
                NSLog(@"error converting payload to json string: %@", error);
            } else {
                NSString *jsonString = [[NSString alloc] initWithData:jsonData encoding:NSUTF8StringEncoding];
                UnitySendMessage(CreateConstChar(@"UserHook"), CreateConstChar(@"handlePayload"), CreateConstChar(jsonString));
            }
        }];
        
        
        
        // Unity post a notification when a remote notification is received. Attach an observer to that notifacation and handle it through User Hook
        [[NSNotificationCenter defaultCenter] addObserver:[UserHookNotification sharedInstance] selector:@selector(handlePushNotification:) name:@"kUnityDidReceiveRemoteNotification" object:nil];
        
        [[NSNotificationCenter defaultCenter] addObserver:[UserHookNotification sharedInstance] selector:@selector(showNewFeedback) name:UH_NotificationNewFeedback object:nil];
        
    }
    
    void _uhDisplayFeedback() {
        [UserHook displayFeedback];
    }
    
    void _uhRateThisApp() {
        [UserHook rateThisApp];
    }
    
    void _uhFetchHookPoints() {
        [UserHook fetchHookPoint:^(UHHookPoint *hookpoint) {
            if(hookpoint) {
                [hookpoint execute];
            }
        }];
    }
    
    void _uhSetFeedbackScreenTitle(const char * title) {
        [UserHook setFeedbackScreenTitle:CreateNSString(title)];
    }
    
    void _uhSetFeedbackCustomFields(const char * fieldString) {
        
        NSString * jsonString = CreateNSString(fieldString);
        
        if (jsonString) {
            
            NSError * error;
            NSData *objectData = [jsonString dataUsingEncoding:NSUTF8StringEncoding];
            NSDictionary *params = [NSJSONSerialization JSONObjectWithData:objectData
                                                                   options:NSJSONReadingMutableContainers
                                                                     error:&error];
            if (error) {
                NSLog(@"error converting feedback parameters: %@", [error localizedDescription]);
            }
            else {
                [UserHook setFeedbackCustomFields:params];
            }
        }
        
    }
    
    void _uhDisplayRatePrompt(const char * message, const char * positiveButtonTitle, const char * negativeButtonTitle) {
        [UserHook displayRatePrompt:CreateNSString(message) positiveButtonTitle:CreateNSString(positiveButtonTitle) negativeButtonTitle:CreateNSString(negativeButtonTitle)];
    }
    
    void _uhDisplayFeedbackPrompt(const char * message, const char * positiveButtonTitle, const char * negativeButtonTitle) {
        [UserHook displayFeedbackPrompt:CreateNSString(message) positiveButtonTitle:CreateNSString(positiveButtonTitle) negativeButtonTitle:CreateNSString(negativeButtonTitle)];
    }
    
    void _uhFetchPageNames() {
        
        [UserHook fetchPageNames:^(NSArray *items) {
            
            // convert UHPage items to Dictionary
            NSMutableArray * pages = [NSMutableArray array];
            for (UHPage * page in items) {
                [pages addObject:[page toDictionary]];
            }
            
            NSError *error;
            NSData *jsonData = [NSJSONSerialization dataWithJSONObject:pages
                                                               options:0
                                                                 error:&error];
            
            if (!jsonData) {
                NSLog(@"error converting pages to json string: %@", error);
            } else {
                NSString *jsonString = [[NSString alloc] initWithData:jsonData encoding:NSUTF8StringEncoding];
                UnitySendMessage(CreateConstChar(@"UserHook"), CreateConstChar(@"handleFetchedPageNames"), CreateConstChar(jsonString));
            }
        }];
    }
    
    void _uhDisplayStaticPage(const char * slug, const char * title) {
        [UserHook displayStaticPage:CreateNSString(slug) title:CreateNSString(title)];
    }
    
    void _uhDisplayPrompt(const char * message, const char * button1, const char * button2) {
        
        NSString * button1Json = CreateNSString(button1);
        NSString * button2Json = CreateNSString(button2);
        
        
        UHMessageMetaButton * btn1;
        UHMessageMetaButton * btn2;
        
        if (button1Json != nil) {
            NSError * error;
            NSData *objectData = [button1Json dataUsingEncoding:NSUTF8StringEncoding];
            NSDictionary * dict = [NSJSONSerialization JSONObjectWithData:objectData
                                                                  options:NSJSONReadingMutableContainers
                                                                    error:&error];
            
            btn1 = [UHMessageMetaButton new];
            if ([dict valueForKey:@"title"]) {
                btn1.title = [dict valueForKey:@"title"];
            }
            if ([dict valueForKey:@"onClickGameObject"] && [dict valueForKey:@"onClickFunction"]) {
                [btn1 setClickHandler:^() {
                    
                    UnitySendMessage(CreateConstChar(@"UserHook"), CreateConstChar(@"handleResponse"), CreateConstChar(button1Json));
                }];
            }
        }
        
        if (button2Json != nil) {
            NSError * error;
            NSData *objectData = [button2Json dataUsingEncoding:NSUTF8StringEncoding];
            NSDictionary * dict = [NSJSONSerialization JSONObjectWithData:objectData
                                                                  options:NSJSONReadingMutableContainers
                                                                    error:&error];
            
            btn2 = [UHMessageMetaButton new];
            if ([dict valueForKey:@"title"]) {
                btn2.title = [dict valueForKey:@"title"];
            }
            if ([dict valueForKey:@"onClickGameObject"] && [dict valueForKey:@"onClickFunction"]) {
                [btn2 setClickHandler:^() {
                    
                    UnitySendMessage(CreateConstChar(@"UserHook"), CreateConstChar(@"handleResponse"), CreateConstChar(button2Json));
                }];
            }
        }
        
        
        [UserHook displayPrompt:CreateNSString(message) button1:btn1 button2:btn2];
        
    }
    
    void _uhUpdateCustomFields(const char * fields, const char * handler) {
        
        NSString * fieldsString = CreateNSString(fields);
        NSString * handlerString = CreateNSString(handler);
        
        if (fieldsString) {
            
            NSError * error;
            NSData *objectData = [fieldsString dataUsingEncoding:NSUTF8StringEncoding];
            NSDictionary * dict = [NSJSONSerialization JSONObjectWithData:objectData
                                                                  options:NSJSONReadingMutableContainers
                                                                    error:&error];
            
            
            if (error) {
                NSLog(@"error converting custom fields to dictionary: %@", [error localizedDescription]);
            }
            else {
                
                [UserHook updateCustomFields:dict handler:^(BOOL success) {
                    
                    if (success) {
                        UnitySendMessage(CreateConstChar(@"UserHook"), CreateConstChar(@"handleResponse"), CreateConstChar(handlerString));
                    }
                }];
            }
            
        }
    }
    
    
    void _uhUpdatePurchasedItem(const char * sku, double price, const char * handler) {
        
        NSString * skuString = CreateNSString(sku);
        NSNumber * priceObject = [NSNumber numberWithDouble:price];
        NSString * handlerString = CreateNSString(handler);
        
        if (skuString && priceObject != nil) {
            
            
            [UserHook updatePurchasedItem:skuString forAmount:priceObject handler:^(BOOL success) {
                if (success) {
                    UnitySendMessage(CreateConstChar(@"UserHook"), CreateConstChar(@"handleResponse"), CreateConstChar(handlerString));
                }
            }];
        }
    }
    
    void _uhRegisterPushToken(const char * token) {
        NSString * tokenString = CreateNSString(token);
        
        if (tokenString) {
            [UserHook registerDeviceTokenString:tokenString];
        }
        
    }
    
#if __cplusplus
}
#endif
