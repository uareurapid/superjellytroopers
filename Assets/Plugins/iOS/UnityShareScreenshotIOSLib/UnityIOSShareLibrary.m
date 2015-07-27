//
//  UnityIOSShareLibrary.m
//  UnityIOSShareLibrary
//
//  Created by Paulo Cristo on 21/06/14.
//  Copyright (c) 2014 Paulo Cristo. All rights reserved.
//

#import "UnityIOSShareLibrary.h"

@implementation UnityIOSShareLibrary



//send the image to twitter
+ (void)sendToTwitter:(NSString *)message andImage:(UIImage *) image rootView:(UIViewController *) rootViewController{
    
    
    if ([SLComposeViewController isAvailableForServiceType:SLServiceTypeTwitter]) {
        
        SLComposeViewController *mySLComposerSheet = [SLComposeViewController composeViewControllerForServiceType:SLServiceTypeTwitter];
        
        [mySLComposerSheet setInitialText:message];
        
        if(image!=nil) {
            [mySLComposerSheet addImage:image];
        }
        
        
        [mySLComposerSheet addURL:[NSURL URLWithString:@"https://itunes.apple.com/us/app/alfie-the-angry-alien-saga/id827406444?ls=1&mt=8"]];
        
        [mySLComposerSheet setCompletionHandler:^(SLComposeViewControllerResult result) {
            
            
            NSString *msg;
            switch (result) {
                case SLComposeViewControllerResultCancelled:
                    msg = @"twitter post canceled";
                    
                    break;
                case SLComposeViewControllerResultDone:
                    //NSLog(@"Post to Twitter Sucessful");
                    msg = @"twitter post ok";
                    break;
                    
                default:
                    break;
            }
            
            
            //dismiss view
            [rootViewController dismissViewControllerAnimated:YES completion:nil];
            
            
            
        }];
        
        [rootViewController presentViewController:mySLComposerSheet animated:YES completion:nil];
    }
}

//will send the message to facebook
+ (void)sendToFacebook:(NSString *)message  andImage:(UIImage *) image rootView:(UIViewController *) rootViewController{
    
    
    if ([SLComposeViewController isAvailableForServiceType:SLServiceTypeFacebook]) {
        
        SLComposeViewController *mySLComposerSheet = [SLComposeViewController composeViewControllerForServiceType:SLServiceTypeFacebook];
        
        [mySLComposerSheet setInitialText:message];
        
        if(image!=nil) {
            [mySLComposerSheet addImage:image];
        }
        
        [mySLComposerSheet addURL:[NSURL URLWithString:@"https://itunes.apple.com/us/app/alfie-the-angry-alien-saga/id827406444?ls=1&mt=8"]];
        
        [mySLComposerSheet setCompletionHandler:^(SLComposeViewControllerResult result) {
            
            NSString *msg;
            switch (result) {
                case SLComposeViewControllerResultCancelled:
                    msg = @"facebook post canceled";
                    //NSLog(@"Post to Facebook Canceled");
                    break;
                case SLComposeViewControllerResultDone:
                    //NSLog(@"Post to Facebook Sucessful");
                    msg = @"facebook post ok";
                    
                    break;
                    
                default:
                    break;
            }
            //dismiss it
            [rootViewController dismissViewControllerAnimated:YES completion:nil];
            
        }];
        
        [rootViewController presentViewController:mySLComposerSheet animated:YES completion:nil];
    }
}

//helper function
+ (UIWindow*) getTopApplicationWindow
{
    UIApplication* clientApp = [UIApplication sharedApplication];
    NSArray* windows = [clientApp windows];
    UIWindow* topWindow = nil;
    
    if (windows && [windows count] > 0)
        topWindow = [[clientApp windows] objectAtIndex:0];
    
    return topWindow;
}




// When native code plugin is implemented in .mm / .cpp file, then functions
// should be surrounded with extern "C" block to conform C function naming rules


+(void) TakeScreenshotFromUnity: (const char*) path
{
    
    UIImage *image = [UIImage imageWithContentsOfFile:[NSString stringWithUTF8String:path]];
    
    
    BOOL isFacebookAvailable = NO;
    BOOL isTwitterAvailable = NO;
    
    //facebook
    if ([SLComposeViewController isAvailableForServiceType:SLServiceTypeFacebook]) {
        isFacebookAvailable=YES;
        
    }
    else {
        isFacebookAvailable = NO;
    }
    //twitter
    
    if ([SLComposeViewController isAvailableForServiceType:SLServiceTypeTwitter]) {
        isTwitterAvailable=YES;
    }
    else {
        isTwitterAvailable = NO;
    }
    
    
    //see wich services we have available
    
    UIViewController *rootViewController = [self getTopApplicationWindow].rootViewController;
    
    NSString* theFileName = [[NSString stringWithUTF8String:path] lastPathComponent];
    NSLog(@"Image name is: %@",theFileName);
    if(image!=nil) {
        if(isTwitterAvailable) {
            [self sendToTwitter:@"Check this awesome game" andImage:image rootView:rootViewController];
        }
        else if(isFacebookAvailable) {
            [self sendToFacebook:@"Check this awesome game" andImage:image rootView:rootViewController];
        }
        else {
        //send to email
        [self sendToEmail:@"check this" andImage:image named:theFileName rootView:rootViewController];
        }
    }
}

+ (void)sendToEmail:(NSString *)message  andImage:(UIImage *) image named:(NSString *)imageName rootView:(UIViewController *) rootViewController{
    MFMailComposeViewController *mc = [[MFMailComposeViewController alloc] init];
    mc.mailComposeDelegate = rootViewController;
    [mc setSubject:@"title"];
    [mc setMessageBody:@"message body" isHTML:NO];
    
    
    //Get all the image info
    if(image!=nil && imageName!=nil) {
        
        //"image/jpeg" png
        NSData *imageData = [self getImageInfoData:image named:imageName];
        BOOL isPNG = [self isImagePNG:imageName];
        
        [mc addAttachmentData:imageData mimeType: isPNG ? @"image/png" : @"image/jpeg" fileName:imageName];
    }
    
    // Present mail view controller on screen
    [rootViewController presentViewController:mc animated:YES completion:NULL];
}
//Get data
+ (NSData *) getImageInfoData:(UIImage *)image named:(NSString *)name {
    
    NSData *imageData = nil;
    
    if ([self isImagePNG:name]) {
        imageData = UIImagePNGRepresentation(image);
    }
    else {
        imageData = UIImageJPEGRepresentation(image, 0.7); // 0.7 is JPG quality
    }
    
    return imageData;
    
}

//check is is PNG
+(BOOL) isImagePNG:(NSString *)imageName {
    bool isPNG = true;
    if ([imageName rangeOfString:@".png"].location != NSNotFound) {
        return isPNG;
    }
    else if([imageName rangeOfString:@".jpg"].location != NSNotFound
            || [imageName rangeOfString:@".jpeg"].location != NSNotFound) {
        isPNG = false;
    }
    else {
        isPNG = true;
    }
    return isPNG;
}

@end






    

