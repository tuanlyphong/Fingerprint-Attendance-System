#include <Adafruit_Fingerprint.h>
#include <SoftwareSerial.h>

SoftwareSerial mySerial(2, 3); // RX, TX
Adafruit_Fingerprint finger = Adafruit_Fingerprint(&mySerial);

uint8_t id;
bool isEnrolling = false;
bool isAttending = false;
enum State {  
    WAITING_FOR_FINGER, 
    REMOVING_FINGER, 
    ENROLLING_FIRST_IMAGE, 
    REMOVING_FINGER_AFTER_FIRST_IMAGE,
    WAITING_FOR_SECOND_IMAGE, 
    ENROLLING_SECOND_IMAGE, 
    CREATING_MODEL,
    STORING_MODEL,
    READING_TEMPLATE
};

State state = WAITING_FOR_FINGER;

void setup() {
    Serial.begin(9600);
    while (!Serial); // Wait for serial connection
    delay(100);

    finger.begin(57600);
    delay(1000);

    if (!finger.verifyPassword()) {
        while (1) { delay(1); }
    }
}

void loop() {
    if (isEnrolling) {
        getFingerprintEnroll();
    } else if (isAttending) {
        getFingerprintAttend();
    }
}

void getFingerprintEnroll() {
    static unsigned long lastActionTime = 0;
    const unsigned long interval = 2000; // 2 seconds delay
    const unsigned long removalDelay = 1000; // Reduced to 1 second
    const unsigned long enrollmentDelay = 200; // Reduced to 0.2 second
    const unsigned long modelDelay = 5000; // Reduced to 5 seconds

    int p = -1;

    switch (state) {
        case WAITING_FOR_FINGER:
            lastActionTime = millis();
            state = REMOVING_FINGER;
            break;

        case REMOVING_FINGER:
            if (millis() - lastActionTime >= interval) {
                lastActionTime = millis();
                state = ENROLLING_FIRST_IMAGE;
            }
            break;

        case ENROLLING_FIRST_IMAGE:
            p = finger.getImage();
            if (p == FINGERPRINT_OK) {
                p = finger.image2Tz(1);
                if (p == FINGERPRINT_OK) {
                    lastActionTime = millis();
                    state = REMOVING_FINGER_AFTER_FIRST_IMAGE;
                } else {
                    state = WAITING_FOR_FINGER;
                }
            }
            break;

        case REMOVING_FINGER_AFTER_FIRST_IMAGE:
            if (millis() - lastActionTime >= removalDelay) {
                lastActionTime = millis();
                state = WAITING_FOR_SECOND_IMAGE;
            }
            break;

        case WAITING_FOR_SECOND_IMAGE:
            p = finger.getImage();
            if (p == FINGERPRINT_OK) {
                lastActionTime = millis();
                state = ENROLLING_SECOND_IMAGE;
            }
            break;

        case ENROLLING_SECOND_IMAGE:
            if (millis() - lastActionTime >= enrollmentDelay) {
                p = finger.image2Tz(2);
                if (p == FINGERPRINT_OK) {
                    state = CREATING_MODEL;
                } else {
                    state = WAITING_FOR_FINGER;
                }
            }
            break;

        case CREATING_MODEL:
            p = finger.createModel();
            if (p == FINGERPRINT_OK) {
                lastActionTime = millis();
                state = STORING_MODEL;
            } else {
                state = WAITING_FOR_FINGER;
            }
            break;

        case STORING_MODEL:
            if (millis() - lastActionTime >= modelDelay) {
                p = finger.storeModel(id);
                if (p == FINGERPRINT_OK) {
                    state = READING_TEMPLATE;
                } else {
                    state = WAITING_FOR_FINGER;
                }
            }
            break;

        case READING_TEMPLATE:
            p = finger.loadModel(id);
            p = finger.getModel();
            uint8_t templateBuffer[534];
            memset(templateBuffer, 0xff, 534);

            uint32_t starttime = millis();
            int i = 0;
            while (i < 534 && (millis() - starttime) < 20000) {
                if (mySerial.available()) {
                    templateBuffer[i++] = mySerial.read();
                }
            }

            uint8_t fingerTemplate[512];
            memset(fingerTemplate, 0xff, 512);

            int uindx = 9, index = 0;
            memcpy(fingerTemplate + index, templateBuffer + uindx, 256);
            uindx += 256;
            uindx += 2;
            uindx += 9;
            index += 256;
            memcpy(fingerTemplate + index, templateBuffer + uindx, 256);
            Serial.println("NewEnrollStart");
            for (int i = 0; i < 512; ++i) {
                if (fingerTemplate[i] < 16) Serial.print("0");
                Serial.print(fingerTemplate[i], HEX);
            }
            Serial.println();
            isEnrolling = false;
            break;
    }
}

void getFingerprintAttend() {
    static unsigned long lastActionTime = 0;
    const unsigned long interval = 2000; // 2 seconds delay
    const unsigned long removalDelay = 1000; // Reduced to 1 second
    const unsigned long enrollmentDelay = 200; // Reduced to 0.2 second
    const unsigned long modelDelay = 5000; // Reduced to 5 seconds

    int p = -1;

    switch (state) {
        case WAITING_FOR_FINGER:
            lastActionTime = millis();
            state = REMOVING_FINGER;
            break;

        case REMOVING_FINGER:
            if (millis() - lastActionTime >= interval) {
                lastActionTime = millis();
                state = ENROLLING_FIRST_IMAGE;
            }
            break;

        case ENROLLING_FIRST_IMAGE:
            p = finger.getImage();
            if (p == FINGERPRINT_OK) {
                p = finger.image2Tz(1);
                if (p == FINGERPRINT_OK) {
                    lastActionTime = millis();
                    state = REMOVING_FINGER_AFTER_FIRST_IMAGE;
                } else {
                    state = WAITING_FOR_FINGER;
                }
            }
            break;

        case REMOVING_FINGER_AFTER_FIRST_IMAGE:
            if (millis() - lastActionTime >= removalDelay) {
                lastActionTime = millis();
                state = WAITING_FOR_SECOND_IMAGE;
            }
            break;

        case WAITING_FOR_SECOND_IMAGE:
            p = finger.getImage();
            if (p == FINGERPRINT_OK) {
                lastActionTime = millis();
                state = ENROLLING_SECOND_IMAGE;
            }
            break;

        case ENROLLING_SECOND_IMAGE:
            if (millis() - lastActionTime >= enrollmentDelay) {
                p = finger.image2Tz(2);
                if (p == FINGERPRINT_OK) {
                    state = CREATING_MODEL;
                } else {
                    state = WAITING_FOR_FINGER;
                }
            }
            break;

        case CREATING_MODEL:
            p = finger.createModel();
            if (p == FINGERPRINT_OK) {
                lastActionTime = millis();
                state = STORING_MODEL;
            } else {
                state = WAITING_FOR_FINGER;
            }
            break;

        case STORING_MODEL:
            if (millis() - lastActionTime >= modelDelay) {
                p = finger.storeModel(id);
                if (p == FINGERPRINT_OK) {
                    state = READING_TEMPLATE;
                } else {
                    state = WAITING_FOR_FINGER;
                }
            }
            break;

        case READING_TEMPLATE:
            p = finger.loadModel(id);
            p = finger.getModel();
            uint8_t templateBuffer[534];
            memset(templateBuffer, 0xff, 534);

            uint32_t starttime = millis();
            int i = 0;
            while (i < 534 && (millis() - starttime) < 20000) {
                if (mySerial.available()) {
                    templateBuffer[i++] = mySerial.read();
                }
            }

            uint8_t fingerTemplate[512];
            memset(fingerTemplate, 0xff, 512);

            int uindx = 9, index = 0;
            memcpy(fingerTemplate + index, templateBuffer + uindx, 256);
            uindx += 256;
            uindx += 2;
            uindx += 9;
            index += 256;
            memcpy(fingerTemplate + index, templateBuffer + uindx, 256);
            Serial.println("NewAttendStart");
            for (int i = 0; i < 512; ++i) {
                if (fingerTemplate[i] < 16) Serial.print("0");
                Serial.print(fingerTemplate[i], HEX);
            }
            Serial.println();
            isAttending = false;
            break;
    }
}

void startEnroll() {
    isEnrolling = true; 
    state = WAITING_FOR_FINGER;
}

void startAttend() {
    isAttending = true; 
    state = WAITING_FOR_FINGER;
}

void serialEvent() {
    String command = Serial.readStringUntil('\n');
    if (command.equals("StartEnroll") && !isEnrolling) {
        startEnroll();
    } else if (command.equals("StartAttend") && !isAttending) {
        startAttend();
    }
}