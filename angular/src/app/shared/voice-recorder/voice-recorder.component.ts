import { Component, EventEmitter, Output, OnDestroy, ChangeDetectorRef } from '@angular/core';
import { CommonModule } from '@angular/common';
import { DomSanitizer, SafeUrl } from '@angular/platform-browser';

@Component({
    selector: 'app-voice-recorder',
    standalone: true,
    imports: [CommonModule],
    templateUrl: './voice-recorder.component.html',
    styleUrls: ['./voice-recorder.component.css']
})
export class VoiceRecorderComponent implements OnDestroy {
    @Output() recordingCompleted = new EventEmitter<Blob>();

    isRecording = false;
    mediaRecorder: MediaRecorder | null = null;
    chunks: Blob[] = [];
    audioUrl: SafeUrl | null = null;
    recordingTime = '00:00';
    private startTime = 0;
    private timerInterval: any;
    hasPermission = false;
    errorMessage = '';

    constructor(private sanitizer: DomSanitizer, private cdr: ChangeDetectorRef) { }

    async startRecording() {
        try {
            this.errorMessage = '';
            const stream = await navigator.mediaDevices.getUserMedia({ audio: true });
            this.hasPermission = true;
            this.mediaRecorder = new MediaRecorder(stream);
            this.chunks = [];

            this.mediaRecorder.ondataavailable = (e) => {
                this.chunks.push(e.data);
            };

            this.mediaRecorder.onstop = () => {
                const blob = new Blob(this.chunks, { type: 'audio/wav' }); // or audio/webm
                this.audioUrl = this.sanitizer.bypassSecurityTrustUrl(URL.createObjectURL(blob));
                this.recordingCompleted.emit(blob); // Emit the blob
                this.cdr.detectChanges();
            };

            this.mediaRecorder.start();
            this.isRecording = true;
            this.startTimer();
        } catch (err) {
            console.error('Error accessing microphone:', err);
            this.errorMessage = 'Mikrofona erişilemedi. Lütfen izinleri kontrol edin.';
            this.hasPermission = false;
        }
    }

    stopRecording() {
        if (this.mediaRecorder && this.isRecording) {
            this.mediaRecorder.stop();
            this.isRecording = false;
            this.stopTimer();
            // Stop all tracks to release microphone
            this.mediaRecorder.stream.getTracks().forEach(track => track.stop());
        }
    }

    private startTimer() {
        this.startTime = Date.now();
        this.timerInterval = setInterval(() => {
            const elapsed = Date.now() - this.startTime;
            this.recordingTime = this.formatTime(elapsed);
            this.cdr.detectChanges();
        }, 1000);
    }

    private stopTimer() {
        clearInterval(this.timerInterval);
    }

    private formatTime(ms: number): string {
        const totalSeconds = Math.floor(ms / 1000);
        const minutes = Math.floor(totalSeconds / 60);
        const seconds = totalSeconds % 60;
        return `${this.pad(minutes)}:${this.pad(seconds)}`;
    }

    private pad(num: number): string {
        return num < 10 ? '0' + num : num.toString();
    }

    ngOnDestroy() {
        this.stopTimer();
    }
}
