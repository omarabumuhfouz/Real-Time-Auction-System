import * as React from "react";
import { cn } from "@/lib/utils";
import { IdCard, Loader2 } from "lucide-react";

export interface FileInputWithButtonProps
  extends Omit<React.InputHTMLAttributes<HTMLInputElement>, "type"> {
  onFileSelect?: (file: File | null) => void;
  /** Show a spinner inside the upload button while OCR or another async process runs */
  isProcessing?: boolean;
}

/**
 * FileInputWithButton
 * A custom file input styled like a standard text input but with an inline "Upload" button.
 * Used globally for single file uploads that need to fit within standard form grids.
 * Supports an `isProcessing` state to indicate async work (e.g. OCR scanning).
 */
export const FileInputWithButton = React.forwardRef<HTMLInputElement, FileInputWithButtonProps>(
  ({ className, onFileSelect, placeholder = "Upload your national id", isProcessing = false, ...props }, ref) => {
    const inputRef = React.useRef<HTMLInputElement | null>(null);
    const [fileName, setFileName] = React.useState<string | null>(null);

    // Merge refs
    const setRefs = React.useCallback(
      (node: HTMLInputElement) => {
        inputRef.current = node;
        if (typeof ref === "function") {
          ref(node);
        } else if (ref) {
          ref.current = node;
        }
      },
      [ref]
    );

    const handleButtonClick = () => {
      if (!isProcessing) {
        inputRef.current?.click();
      }
    };

    const handleFileChange = (e: React.ChangeEvent<HTMLInputElement>) => {
      const file = e.target.files?.[0] || null;
      setFileName(file ? file.name : null);
      if (onFileSelect) {
        onFileSelect(file);
      }
      if (props.onChange) {
        props.onChange(e);
      }
    };

    return (
      <div
        className={cn(
          "relative flex items-center h-12 w-full rounded-full border border-input bg-input-background transition-colors",
          "focus-within:outline-none focus-within:ring-2 focus-within:ring-ring focus-within:ring-offset-2",
          className
        )}
      >
        {/* Hidden actual file input */}
        <input
          {...props}
          type="file"
          ref={setRefs}
          onChange={handleFileChange}
          className="hidden"
          accept="image/*,.pdf"
          disabled={isProcessing || props.disabled}
        />

        {/* Visual wrapper mimicking standard input contents */}
        <div className="flex items-center w-full h-full pl-4 pr-1">
          <div className="text-muted-foreground mr-3">
            <IdCard className="h-5 w-5" />
          </div>

          <div className="flex-1 truncate text-sm text-muted-foreground">
            {fileName ? <span className="text-foreground">{fileName}</span> : placeholder}
          </div>

          <button
            type="button"
            onClick={handleButtonClick}
            disabled={isProcessing}
            className={cn(
              "h-10 px-6 rounded-full text-sm font-medium transition-colors flex items-center gap-2",
              isProcessing
                ? "bg-primary/60 text-primary-foreground cursor-not-allowed"
                : "bg-primary hover:bg-primary/90 text-primary-foreground"
            )}
          >
            {isProcessing ? (
              <>
                <Loader2 className="h-4 w-4 animate-spin" />
                <span>Scanning…</span>
              </>
            ) : (
              "Upload"
            )}
          </button>
        </div>
      </div>
    );
  }
);
FileInputWithButton.displayName = "FileInputWithButton";
