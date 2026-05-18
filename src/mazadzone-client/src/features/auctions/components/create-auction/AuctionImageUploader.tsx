"use client";

import { useState } from "react";
import { Upload, X } from "lucide-react";

interface ImagePreview {
  id: string;
  url: string;
  file: File;
}

interface AuctionImageUploaderProps {
  imagePreviews: ImagePreview[];
  onFilesSelected: (files: FileList) => void;
  onImageRemoved: (id: string) => void;
  error?: string;
}

export function AuctionImageUploader({
  imagePreviews,
  onFilesSelected,
  onImageRemoved,
  error,
}: AuctionImageUploaderProps) {
  const [dragActive, setDragActive] = useState(false);

  const handleDrag = (e: React.DragEvent) => {
    e.preventDefault();
    e.stopPropagation();
    if (e.type === "dragenter" || e.type === "dragover") {
      setDragActive(true);
    } else if (e.type === "dragleave") {
      setDragActive(false);
    }
  };

  const handleDrop = (e: React.DragEvent) => {
    e.preventDefault();
    e.stopPropagation();
    setDragActive(false);
    if (e.dataTransfer.files && e.dataTransfer.files[0]) {
      onFilesSelected(e.dataTransfer.files);
    }
  };

  return (
    <div className="space-y-3">
      <h3 className="text-lg font-bold text-foreground tracking-tight">
        Auction Images
      </h3>
      
      {/* Dropzone */}
      <div
        onDragEnter={handleDrag}
        onDragOver={handleDrag}
        onDragLeave={handleDrag}
        onDrop={handleDrop}
        onClick={() => document.getElementById("file-upload")?.click()}
        className={`border-2 border-dashed rounded-xl p-8 flex flex-col items-center justify-center gap-3 transition-colors cursor-pointer min-h-[180px] bg-muted/5 ${
          dragActive 
            ? "border-primary bg-primary/5" 
            : error 
            ? "border-red-500/50 hover:border-red-500" 
            : "border-border hover:border-primary/50"
        }`}
      >
        <input
          id="file-upload"
          type="file"
          multiple
          accept="image/*"
          className="hidden"
          onChange={(e) => e.target.files && onFilesSelected(e.target.files)}
        />
        
        <div className="flex h-12 w-12 items-center justify-center rounded-full bg-primary/10 text-primary">
          <Upload className="h-6 w-6 stroke-[2.2]" />
        </div>
        
        <div className="space-y-1 text-center">
          <p className="font-bold text-foreground text-[15px]">
            Click to upload images
          </p>
          <p className="text-xs text-muted-foreground font-semibold">
            PNG, JPG up to 5MB (Max 10 images)
          </p>
        </div>
      </div>

      {/* Error Message */}
      {error && (
        <p className="text-xs text-red-500 font-bold tracking-tight">{error}</p>
      )}

      {/* Image Preview Grid */}
      {imagePreviews.length > 0 && (
        <div className="grid grid-cols-2 sm:grid-cols-4 md:grid-cols-5 gap-3 pt-2">
          {imagePreviews.map((preview) => (
            <div 
              key={preview.id} 
              className="group relative aspect-square rounded-lg overflow-hidden border border-border bg-muted shadow-sm"
            >
              {/* eslint-disable-next-line @next/next/no-img-element */}
              <img 
                src={preview.url} 
                alt="Preview" 
                className="h-full w-full object-cover transition-transform duration-300 group-hover:scale-105"
              />
              <div className="absolute inset-0 bg-black/40 opacity-0 group-hover:opacity-100 transition-opacity flex items-center justify-center">
                <button
                  type="button"
                  onClick={(e) => {
                    e.stopPropagation();
                    onImageRemoved(preview.id);
                  }}
                  className="h-8 w-8 rounded-full bg-red-600 hover:bg-red-700 text-white flex items-center justify-center cursor-pointer transition-colors shadow"
                >
                  <X className="h-4.5 w-4.5 stroke-3" />
                </button>
              </div>
            </div>
          ))}
        </div>
      )}
    </div>
  );
}
