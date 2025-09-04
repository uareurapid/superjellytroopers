//
//  ShareScreenshotWrapper.c
//  UnityIOSShareLibrary
//
//  Created by Paulo Cristo on 22/06/14.
//  Copyright (c) 2014 Paulo Cristo. All rights reserved.
//

#import "ShareScreenshotWrapper.h"
#import "UnityIOSShareLibrary.h"



void _TakeScreenshot(const char *path) {
    [UnityIOSShareLibrary TakeScreenshotFromUnity:path];
}