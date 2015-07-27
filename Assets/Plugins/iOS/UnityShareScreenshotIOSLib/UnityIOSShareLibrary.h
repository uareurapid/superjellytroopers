//
//  UnityIOSShareLibrary.h
//  UnityIOSShareLibrary
//
//  Created by Paulo Cristo on 21/06/14.
//  Copyright (c) 2014 Paulo Cristo. All rights reserved.
//

#import <Foundation/Foundation.h>
#import <UIKit/UIKit.h>
#import <Social/Social.h>
#import <UIKit/UIKit.h>
#import <MessageUI/MessageUI.h>
#import <MessageUI/MFMailComposeViewController.h>

@interface UnityIOSShareLibrary : NSObject <MFMailComposeViewControllerDelegate,MFMessageComposeViewControllerDelegate>



+(void) TakeScreenshotFromUnity:(const char *) path;
+ (UIWindow*) getTopApplicationWindow;

@end
